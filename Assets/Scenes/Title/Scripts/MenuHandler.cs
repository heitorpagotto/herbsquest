using UnityEngine;
using UnityEngine.EventSystems;

public class MenuHandler : MonoBehaviour
{
    [Header("Canvas")] 
    [SerializeField] 
    private GameObject mainMenuCanvas;
    [SerializeField]
    private GameObject optionsMenuCanvas;
    
    [Header("First selected options")]
    [SerializeField]
    private GameObject mainMenuFirstOption;
    [SerializeField]
    private GameObject optionsMenuFirstOption;
    
    void Start()
    {
        OpenMainMenu();
    }

    public void OpenSettingsMenu()
    {
        mainMenuCanvas.SetActive(false);
        optionsMenuCanvas.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(optionsMenuFirstOption);
    }

    public void OpenMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        optionsMenuCanvas.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(mainMenuFirstOption);
    }

    public void DeleteSavedGame()
    {
        GameSave.DeleteSave();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
