using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

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
        Vector2 movement = new Vector2(direction * speed * Time.fixedDeltaTime, 0f);
        
        _rigidbody.MovePosition(_rigidbody.position + movement);
        
        bool hitWall= Physics2D.OverlapCircle(wallCheck.position, checkRadius, levelLayer);

        // Debug.Log(Physics2D.Raycast(transform.position, facingDirection, .1f, levelLayer).distance);
        
        if (hitWall)
            FlipDirection();
        
        // if (Physics2D.Raycast(transform.position, facingDirection, .1f, levelLayer))
        //     FlipDirection();
    }

    void FlipDirection()
    {
        _facingRight = !_facingRight;
        
        Vector3 wPos = wallCheck.localPosition;
        wPos.x *= -1f;
        wallCheck.localPosition = wPos;
        
        _spriteRenderer.flipX = _facingRight;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        var contactPoint = other.GetContact(0);
        var playerHealth = other.gameObject.GetComponent<PlayerInventory>();
        var playerController = other.gameObject.GetComponent<PlayerController>();

        Debug.Log(contactPoint.normal);
        
        if (contactPoint.normal.y < -0.5f)
        {
             OnEnemyHit(playerController);
        }
        else
        {
            playerHealth.TakeDamage();
        }
    }
    
    protected abstract void OnEnemyHit(PlayerController controller);
}
