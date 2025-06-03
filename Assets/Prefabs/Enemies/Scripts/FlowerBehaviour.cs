using System.Collections;
using UnityEngine;

public class FlowerBehaviour : EnemyBehaviour
{
    [SerializeField] private int hitsToKill = 2;

    private int _hits = 0;
    protected override void OnEnemyHit(PlayerController controller)
    {
        _hits++;

        controller.Bounce(25f);

        // var rigidBody = gameObject.GetComponent<Rigidbody2D>();
        // rigidBody.linearVelocity = Vector2.zero;

        var shouldDie = _hits >= hitsToKill;
        StartCoroutine(PlayParticle(shouldDie ? 3 : 2, shouldDie));
    }

    IEnumerator PlayParticle(int position, bool dead = false)
    {
        if (dead)
        {
            var sprite = gameObject.GetComponent<SpriteRenderer>();
            sprite.enabled = false;
            AudioManager.Instance.PlaySfx("EnemyDead");
        }
        
        if (!dead)
            AudioManager.Instance.PlaySfx("EnemyHit");
        
        var particle = gameObject.transform.GetChild(position).GetComponent<ParticleSystem>();
        
        particle.Play();
        
        yield return new WaitUntil(() => !particle.IsAlive(true));
        
        if (dead)
            Destroy(gameObject);
    }
}
