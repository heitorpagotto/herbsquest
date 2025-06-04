using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    
    public bool MenuOpenCloseInput { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
