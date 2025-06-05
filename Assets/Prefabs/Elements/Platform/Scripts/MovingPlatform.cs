using JetBrains.Annotations;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("— Waypoints (in World Space) —")]
    [Tooltip("Transforms marking each point.  At least two required.")]
    public Transform[] waypoints;

    [Tooltip("Units per second that the platform moves.")]
    public float speed = 2f;

    [Tooltip("If true, the platform moves A→B→A→B… (ping‐pong).")]
    public bool pingPong = true;

    // [Header('For circular path')]
    // public bool useCircularPath;
    // public Transform circleCenter;
    // public float radius = 2f;
    // public float angularSpeed = 90f; // degrees per second

    private int _currentIndex = 0;
    private bool _headingForward = true;
    private Vector3 _nextTarget;
    private Collider2D _col2D;
    [CanBeNull]
    private FallingPlatformBehaviour _fallingPlatform;

    private bool _shouldMove = true;

    void Awake()
    {
        _col2D = GetComponent<Collider2D>();
        _fallingPlatform = GetComponent<FallingPlatformBehaviour>();

        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogError($"[{name}] You must assign at least two waypoints in the inspector.");
            enabled = false;
            return;
        }

        // Start at the first waypoint
        transform.position = waypoints[0].position;
        ComputeNextTarget();
    }
    
    // void OnBecameVisible()
    // {
    //     _shouldMove = true;
    // }
    //
    // void OnBecameInvisible()
    // {
    //     _shouldMove = false;
    // }

    void FixedUpdate()
    {
        if (_fallingPlatform != null && _fallingPlatform.hasDropped || !_shouldMove) return;
        
        MovePlatform();
    }

    void MovePlatform()
    {
        // Move toward _nextTarget
        Vector3 currentPos = transform.position;
        Vector3 dir = (_nextTarget - currentPos).normalized;
        float step = speed * Time.deltaTime;

        // If we’re very close, snap to target, then pick the next
        if (Vector3.Distance(currentPos, _nextTarget) <= step)
        {
            transform.position = _nextTarget;
            AdvanceIndex();
            ComputeNextTarget();
        }
        else
        {
            transform.position += dir * step;
        }
    }
 
    private void ComputeNextTarget()
    {
        // Just look up the position of the waypoint at _currentIndex
        _nextTarget = waypoints[_currentIndex].position;
    }

    private void AdvanceIndex()
    {
        if (pingPong)
        {
            if (_headingForward)
            {
                _currentIndex++;
                if (_currentIndex >= waypoints.Length)
                {
                    // Hit the end; reverse
                    _headingForward = false;
                    _currentIndex = waypoints.Length - 2; // step back one
                }
            }
            else // heading backward
            {
                _currentIndex--;
                if (_currentIndex < 0)
                {
                    // Hit the start; reverse
                    _headingForward = true;
                    _currentIndex = 1; // step forward one
                }
            }
        }
        else
        {
            // Simple loop: once we pass final waypoint, wrap to zero
            _currentIndex = (_currentIndex + 1) % waypoints.Length;
        }
    }

    // ------------------------------------------------
    // Parenting logic so that “Player” rides along
    // ------------------------------------------------

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Only parent if the player lands from roughly above
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("test");
            foreach (var cp in collision.contacts)
            {
                if (cp.normal.y <= -0.5f)
                {
                    // Parent the player’s transform to this platform
                    collision.collider.transform.parent.SetParent(this.transform);
                    break;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Once the player leaves, un‐parent
        if (collision.collider.CompareTag("Player"))
            collision.collider.transform.parent.SetParent(null);
    }
}
