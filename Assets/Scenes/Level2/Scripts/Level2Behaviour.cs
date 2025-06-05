using UnityEngine;

public class Level2Behaviour : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance?.PlayMusic("Level2Theme", 0f, true);        
    }
}
