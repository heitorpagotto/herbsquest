using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    
    public bool PauseOpenClose { get; private set; }

    private PlayerInput _playerInput;
    private InputAction _pauseOpenCloseAction;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        _playerInput = GetComponent<PlayerInput>();
        _pauseOpenCloseAction = _playerInput.actions["PauseOpenClose"];
    }

    void Update()
    {
        PauseOpenClose = _pauseOpenCloseAction.WasPressedThisFrame();
    }
}
