using System.Collections;
using UnityEngine;

public class KoopaBehaviour : EnemyBehaviour
{
    private bool _inShell = false;

    protected override void OnEnemyHit(PlayerController controller)
    {
        controller.Bounce(25f);

        if (!_inShell)
        {
            var animator = gameObject.GetComponent<Animator>();
            _inShell = true;
            var rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Static;
            
            animator.SetBool("inShell", true);
            StopPatrol();
            StartCoroutine(PlayParticle(false));
        }
        else
        {
            StartCoroutine(PlayParticle(true));
        }
    }
    
    IEnumerator PlayParticle(bool die)
    {
        if (!die)
        {
            var hitParticle = gameObject.transform.GetChild(2).GetComponent<ParticleSystem>();
            hitParticle.Play();    
            
            AudioManager.Instance?.PlaySfx("EnemyHit");
            
            yield return new WaitUntil(() => !hitParticle.IsAlive(true));
        }
        else
        {
            var sprite = gameObject.GetComponent<SpriteRenderer>();
            var colliderForKoopa = GetComponent<Collider2D>();

            sprite.enabled = false;
            colliderForKoopa.enabled = false;
        
            var smokeParticle = gameObject.transform.GetChild(3).GetComponent<ParticleSystem>();
        
            AudioManager.Instance?.PlaySfx("EnemyDead");
        
            smokeParticle.Play();
            
            yield return new WaitUntil(() => !smokeParticle.IsAlive(true));
        
            Destroy(gameObject);
        }
    }
}
