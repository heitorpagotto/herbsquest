using System.Collections;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    public void Collect()
    {
        var coinSprite = gameObject.transform.GetChild(0).gameObject;
        coinSprite.gameObject.SetActive(false);

        var particle = gameObject.transform.GetChild(1).GetComponent<ParticleSystem>();
        
        particle.Play();
        
        // handle sound

        StartCoroutine(ParticlesAlive(particle));
    }

    private IEnumerator ParticlesAlive(ParticleSystem particle)
    {
        yield return new WaitUntil(() => !particle.IsAlive(true));
        
        gameObject.SetActive(false);
    }
}
