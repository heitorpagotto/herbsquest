using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Hey!
/// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
/// I have a premium version on Patreon, which has every feature you'd expect from a polished controller. Link: https://www.patreon.com/tarodev
/// You can play and compete for best times here: https://tarodev.itch.io/extended-ultimate-2d-controller
/// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/tarodev
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField]
    private ScriptableStats stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    #region Interface

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
    public event Action Flutter;

    #endregion

    private float _time;
    
    private bool _isHurt = false;
    private bool _isInvincible = false;
    [SerializeField] private float hurtDuration = 1f;
    [SerializeField] private float invincibleDuration = 0.5f;
    [SerializeField] private float knockbackHorizontalForce = 10f;
    [SerializeField] private float knockbackVerticalForce = 5f;

// ── Expose for other scripts:
    public bool IsInvincible => _isInvincible;
    public bool IsGrounded => _isHurt;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
    }

    private void GatherInput()
    {
        if (_isHurt)
        {
            _frameInput = new FrameInput {
                JumpDown = false,
                JumpHeld = false,
                Move = Vector2.zero,
                Crouch = false
            };
            return;
        }
        
        var analogH = Input.GetAxisRaw("Horizontal");
        var analogV = Input.GetAxisRaw("Vertical");
        
        // 2) Read your new D-pad axes:
        float dpadH  = Input.GetAxisRaw("DPadHorizontal");  // -1,0,+1 from D-pad Left/Right
        float dpadV  = Input.GetAxisRaw("DPadVertical");    // -1,0,+1 from D-pad Down/Up

        // 3) Combine them. Clamp to [-1, +1] so you don’t exceed max:
        float horizontal = Mathf.Clamp(analogH + dpadH, -1f, 1f);
        float vertical   = Mathf.Clamp(analogV + dpadV, -1f, 1f);
        
        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
            Move = new Vector2(horizontal, vertical),
            Crouch = Input.GetAxisRaw("Vertical") < 0
        };

        if (stats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < stats.HorizontalDeadZoneThreshold
                ? 0
                : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < stats.VerticalDeadZoneThreshold
                ? 0
                : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();

        if (_isHurt) return;

        HandleJump();
        HandleDirection();
        HandleGravity();

        ApplyMovement();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!_frameInput.Crouch) return;
        
        foreach (var contact in other.contacts)
        {
            var obj = contact.collider.gameObject;

            if (!obj.CompareTag("Pipe")) continue;
            
            var pipe = obj.GetComponent<PipeBehaviour>();
            pipe.Teleport(gameObject);
        }
    }
    

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down,
            stats.GrounderDistance, ~stats.PlayerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up,
            stats.GrounderDistance, ~stats.PlayerLayer);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }
        
        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion


    #region Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }

    public void Bounce(float jumpPower)
    {
        ExecuteJump(false, jumpPower);
    }

    private void ExecuteJump(bool playSfx = true, float? jumpPower = null)
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = jumpPower ?? stats.JumpPower;
        Jumped?.Invoke();
        
        if (playSfx)
            AudioManager.Instance?.PlaySfx("PlayerJump");
    }

    #endregion

    #region Horizontal

    private void HandleDirection()
    {
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? stats.GroundDeceleration : stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * stats.MaxSpeed,
                stats.Acceleration * Time.fixedDeltaTime);

            _spriteRenderer.flipX = _frameVelocity.x < 0;
        }
        
        _animator.SetFloat("speed", Mathf.Abs(_frameVelocity.x));
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = stats.GroundingForce;
            _animator.SetBool("jumping", false);
            _animator.SetBool("falling", false);
        }
        else
        {
            var inAirGravity = stats.FallAcceleration;
            _animator.SetBool("jumping", true);
            if (_endedJumpEarly && _frameVelocity.y > 0)
            {
                inAirGravity *= stats.JumpEndEarlyGravityModifier;
                _animator.SetBool("falling", true);
                _animator.SetBool("jumping", false);
            }
            _frameVelocity.y =
                Mathf.MoveTowards(_frameVelocity.y, -stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;
    
    public void GetHurt(Vector2 enemyPosition)
    {
        if (_isInvincible || _isHurt) return;
        StartCoroutine(HurtCoroutine(enemyPosition));
    }

    private IEnumerator HurtCoroutine(Vector2 enemyPosition)
    {
        // 1) trigger hurt animation + sound
        _animator.SetTrigger("hurt");
        AudioManager.Instance?.PlaySfx("PlayerHurt");

        // 2) freeze frame for 0.05 real seconds
        float prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = prevTimeScale;

        _isInvincible = true;
        
        // 3) knockback if grounded
        if (_grounded)
        {
            Vector2 dir = ((Vector2)transform.position - enemyPosition).normalized;
            _rb.linearVelocity = new Vector2(
                dir.x * knockbackHorizontalForce,
                knockbackVerticalForce
            );
            _isHurt = true;

            // hold “lost control” for exactly hurtDuration
            yield return new WaitForSeconds(hurtDuration);
            _isHurt = false;
        }
        else
        {
            // In air: skip knockback & skip control loss, 
            // but we still did the freeze + hurt animation above.
            _isHurt = false;
        }

        // 4) short invincibility
        yield return new WaitForSeconds(invincibleDuration);
        _isInvincible = false;
    }
    
    

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (stats == null)
            Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif
}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
    public bool Crouch;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    public event Action Jumped;
    public event Action Flutter;
    public Vector2 FrameInput { get; }
}