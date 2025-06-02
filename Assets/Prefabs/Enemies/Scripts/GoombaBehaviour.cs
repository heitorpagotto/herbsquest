using System.Collections;
using UnityEngine;

public class GoombaBehaviour : EnemyBehaviour
{
    protected override void OnEnemyHit(PlayerController controller)
    {
        controller.Bounce(25f);

        StartCoroutine(PlayParticle(2, false));
    }

    IEnumerator PlayParticle(int position, bool dead = false)
    {
        if (dead)
        {
            var sprite = gameObject.GetComponent<SpriteRenderer>();
            sprite.enabled = false;
        }
        
        var particle = gameObject.transform.GetChild(position).GetComponent<ParticleSystem>();
        
        particle.Play();
        
        yield return new WaitUntil(() => !particle.IsAlive(true));
        
        if (dead)
            Destroy(gameObject);
        else
            StartCoroutine(PlayParticle(3, true));
    }
}
