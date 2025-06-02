using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    
    [SerializeField]
    private Button deleteSaveDataButton;

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
        deleteSaveDataButton.interactable = GameSave.HasSaveData();

        mainMenuCanvas.SetActive(true);
        optionsMenuCanvas.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(mainMenuFirstOption);
    }

    public void DeleteSavedGame()
    {
        GameSave.DeleteSave();
        deleteSaveDataButton.interactable = false;
        EventSystem.current.SetSelectedGameObject(mainMenuFirstOption);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Map", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
