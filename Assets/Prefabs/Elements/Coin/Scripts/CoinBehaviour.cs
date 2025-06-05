using System.Collections;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    public void Collect()
    {
        var coinSprite = gameObject.transform.GetChild(0).gameObject;
        coinSprite.gameObject.SetActive(false);

        var boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;

        var particle = gameObject.transform.GetChild(1).GetComponent<ParticleSystem>();
        
        particle.Play();
        
        AudioManager.Instance?.PlaySfx("CoinCollect");

        StartCoroutine(ParticlesAlive(particle));
    }

    private IEnumerator ParticlesAlive(ParticleSystem particle)
    {
        yield return new WaitUntil(() => !particle.IsAlive(true));
        
        Destroy(gameObject);
    }
}
