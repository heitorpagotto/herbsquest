using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider2D))]
public class SpringBehaviour : MonoBehaviour
{
    [SerializeField] private float springBoostPower = 30f;

    private Animator _animator;
    
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var controller = other.gameObject.GetComponent<PlayerController>();
            
            AudioManager.Instance?.PlaySfx("Spring");
            
            controller.Bounce(springBoostPower); 
        }
    }
}
