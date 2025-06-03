using UnityEngine;

public class Level1Behaviour : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance?.PlayMusic("Level1Theme", 10f);
    }
}
