using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private bool avoidPits = false;

    [Header("Environment Check")] 
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField] 
    private LayerMask levelLayer;
    [SerializeField]
    private float checkRadius = 0.1f;

    
    private Rigidbody2D _rigidbody;
    private bool _facingRight;
    private SpriteRenderer _spriteRenderer;
    private bool _shouldPatrol;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (_shouldPatrol)
            Patrol();
    }
    
    void OnBecameVisible()
    {
        _shouldPatrol = true;
    }

    void OnBecameInvisible()
    {
        _shouldPatrol = false;
    }

    void Patrol()
    {
        float direction = _facingRight ? 1 : -1;
        
        _rigidbody.linearVelocity = new Vector2(direction * speed, _rigidbody.linearVelocity.y);
        
        bool hitWall= Physics2D.OverlapCircle(wallCheck.position, checkRadius, levelLayer);
        bool noGround = !Physics2D.OverlapCircle(groundCheck.position, checkRadius, levelLayer) && avoidPits;

        if (hitWall || noGround)
            FlipDirection();
    }

    public void StopPatrol()
    {
        _shouldPatrol = false;
    }

    public void StartPatrol()
    {
        _shouldPatrol = true;
    }

    void FlipDirection()
    {
        _facingRight = !_facingRight;
        
        Vector3 wPos = wallCheck.localPosition;
        wPos.x *= -1f;
        wallCheck.localPosition = wPos;
        
        Vector3 gPos = groundCheck.localPosition;
        gPos.x *= -1f;
        groundCheck.localPosition = gPos;
        
        _spriteRenderer.flipX = _facingRight;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        var contactPoint = other.GetContact(0);
        var playerHealth = other.gameObject.GetComponent<PlayerInventory>();
        var playerController = other.gameObject.GetComponent<PlayerController>();

        if (contactPoint.normal.y < -0.5f)
        {
             OnEnemyHit(playerController);
        }
        else
        {
            playerHealth.TakeDamage(transform.position);
        }
    }
    
    protected abstract void OnEnemyHit(PlayerController controller);
}
