using UnityEngine;

public class Level3Behaviour : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance?.PlayMusic("Level3Theme", 0f, true);
    }
}
