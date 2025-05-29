using System.Collections;
using UnityEngine;

public class HeartUpgradeBehaviour : MonoBehaviour
{
    private Transform _heartSpriteObject;   
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _heartSpriteObject = gameObject.transform.GetChild(0);
    }

    public void Collect()
    {
        _heartSpriteObject.gameObject.SetActive(false);

        var particle = gameObject.transform.GetChild(1).GetComponent<ParticleSystem>();
        
        particle.Play();

        StartCoroutine(ParticlesAlive(particle));
    }
    
    private IEnumerator ParticlesAlive(ParticleSystem particle)
    {
        yield return new WaitUntil(() => !particle.IsAlive(true));
        
        gameObject.SetActive(false);
    }
}
