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
    
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Slider sfxSlider;

    void Start()
    {
        OpenMainMenu(false);
        
        AudioManager.Instance.PlayMusic("MainTheme", 30f);
        
        musicSlider.value = AudioManager.Instance?.musicSource.volume ?? 0.5f;
        sfxSlider.value = AudioManager.Instance?.sfxSource.volume ?? 0.5f;
    }

    public void OpenSettingsMenu()
    {
        AudioManager.Instance.PlaySfx("Selection");
        mainMenuCanvas.SetActive(false);
        optionsMenuCanvas.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(optionsMenuFirstOption);
    }

    public void ChangeMusicValue()
    {
        AudioManager.Instance.ChangeMusicVolume(musicSlider.value);
    }
    
    public void ChangeSfxValue()
    {
        AudioManager.Instance.ChangeSfxVolume(sfxSlider.value);
    }

    public void OpenMainMenu(bool playSfx = true)
    {
        if (playSfx)
            AudioManager.Instance.PlaySfx("Selection");
        
        deleteSaveDataButton.interactable = GameSave.HasSaveData();

        mainMenuCanvas.SetActive(true);
        optionsMenuCanvas.SetActive(false);
        
        EventSystem.current.SetSelectedGameObject(mainMenuFirstOption);
    }

    public void DeleteSavedGame()
    {
        AudioManager.Instance.PlaySfx("Selection");
        GameSave.DeleteSave();
        deleteSaveDataButton.interactable = false;
        EventSystem.current.SetSelectedGameObject(mainMenuFirstOption);
    }

    public void StartGame()
    {
        AudioManager.Instance.PlaySfx("Selection");
        AudioManager.Instance.musicSource.Stop();
        SceneManager.LoadScene("Map", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlaySfx("Selection");
        Application.Quit();
    }
}
