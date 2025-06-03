using System.Collections;
using UnityEngine;

public class GoombaBehaviour : EnemyBehaviour
{
    protected override void OnEnemyHit(PlayerController controller)
    {
        controller.Bounce(25f);
        StartCoroutine(PlayParticle());
    }

    IEnumerator PlayParticle()
    {
        var hitParticle = gameObject.transform.GetChild(2).GetComponent<ParticleSystem>();
        hitParticle.Play();
        
        var sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        
        var smokeParticle = gameObject.transform.GetChild(3).GetComponent<ParticleSystem>();
        
        AudioManager.Instance?.PlaySfx("EnemyDead");
        
        smokeParticle.Play();
        
        yield return new WaitUntil(() => !smokeParticle.IsAlive(true));
        
        Destroy(gameObject);
    }
}
