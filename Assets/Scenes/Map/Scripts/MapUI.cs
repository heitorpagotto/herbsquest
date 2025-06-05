using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [Header("Pause Screen")] 
    [SerializeField]
    private GameObject pauseParent;
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject pauseOptionsScreen;
    [SerializeField]
    private GameObject pauseFirstOption;
    [SerializeField]
    private GameObject pauseOptionsFirstOption;
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Slider sfxSlider;
    
    private bool _isPaused;

    void Start()
    {
        musicSlider.value = AudioManager.Instance?.musicSource.volume ?? 0.5f;
        sfxSlider.value = AudioManager.Instance?.sfxSource.volume ?? 0.5f;
    }
    
    void Update()
    {
        if (PauseManager.Instance.PauseOpenClose)
        {
            if (!_isPaused)
            {
                AudioManager.Instance?.musicSource.Pause();
                AudioManager.Instance?.PlaySfx("Pause");
                Pause();
            }
            else
            {
                Unpause();
            }
        }
    }
    
    void Pause()
    {
        _isPaused = true;
        pauseParent.SetActive(true);
        OpenPauseMenu();
        Time.timeScale = 0f;
    }

    public void Unpause()
    {
        _isPaused = false;
        pauseParent.SetActive(false);
        pauseScreen.SetActive(false);
        pauseOptionsScreen.SetActive(false);
        Time.timeScale = 1f;
        AudioManager.Instance?.musicSource.UnPause();
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    public void OpenPauseMenu()
    {
        pauseScreen.SetActive(true);
        pauseOptionsScreen.SetActive(false);
        EventSystem.current.SetSelectedGameObject(pauseFirstOption);

    }

    public void OpenSettings()
    {
        pauseScreen.SetActive(false);
        pauseOptionsScreen.SetActive(true);
        
        EventSystem.current.SetSelectedGameObject(pauseOptionsFirstOption);
    }

    public void Exit()
    {
        Unpause();
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }
    
    public void ChangeMusicValue()
    {
        AudioManager.Instance?.ChangeMusicVolume(musicSlider.value);
    }
    
    public void ChangeSfxValue()
    {
        AudioManager.Instance?.ChangeSfxVolume(sfxSlider.value);
    }
}
