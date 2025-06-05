using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private PlayerInventory inventoryScript;
    [SerializeField] private Sprite[] healthSprites;
    [SerializeField] private EndLevelBehaviour endLevelBehaviour;

    [Header("Pause Screen")] [SerializeField]
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCoinUI();
        UpdateEmptyHealth();
        UpdateFilledHealth();

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
    
    public void ChangeMusicValue()
    {
        AudioManager.Instance?.ChangeMusicVolume(musicSlider.value);
    }
    
    public void ChangeSfxValue()
    {
        AudioManager.Instance?.ChangeSfxVolume(sfxSlider.value);
    }

    public void ExitLevel()
    {
        Unpause();
        SceneManager.LoadScene("Map", LoadSceneMode.Single);
    }

    private void OnEnable()
    {
        inventoryScript.OnCoinChange += UpdateCoinUI;
        inventoryScript.OnMaxHealthChange += UpdateEmptyHealth;
        inventoryScript.OnCurrentHealthChange += UpdateFilledHealth;
        inventoryScript.OnPlayerDeath += PlayerDead;
        endLevelBehaviour.OnLevelEnd += EndLevel;
    }

    private void OnDisable()
    {
        inventoryScript.OnCoinChange -= UpdateCoinUI;
        inventoryScript.OnMaxHealthChange -= UpdateEmptyHealth;
        inventoryScript.OnCurrentHealthChange -= UpdateFilledHealth;
        inventoryScript.OnPlayerDeath -= PlayerDead;
        endLevelBehaviour.OnLevelEnd -= EndLevel;
    }

    void UpdateCoinUI()
    {
        coinText.text = inventoryScript.Coins.ToString("D2");
    }

    void UpdateEmptyHealth()
    {
        var parentObj = gameObject.transform.GetChild(0).GetChild(0).transform;
        
        for (int i = parentObj.childCount - 1; i >= 0; i--)
        {
            GameObject child = parentObj.GetChild(i).gameObject;
            Destroy(child);
        }
        
        for (int i = 0; i < inventoryScript.MaxHealth; i++)
        {
            var image = new GameObject($"empty-heart-{i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

            image.transform.SetParent(parentObj, false);
            
            var rt = image.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            
            rt.sizeDelta = new Vector2(80,80);

            rt.anchoredPosition = new Vector2(80 * i, 0f);

            var img = image.GetComponent<Image>();
            img.sprite = healthSprites[0];
            img.preserveAspect = true;
        }
    }
    
    void UpdateFilledHealth()
    {
        var parentObj = gameObject.transform.GetChild(0).GetChild(1).transform;
        
        for (int i = parentObj.childCount - 1; i >= 0; i--)
        {
            GameObject child = parentObj.GetChild(i).gameObject;
            Destroy(child);
        }
        
        for (int i = 0; i < inventoryScript.CurrentHealth; i++)
        {
            var image = new GameObject($"filled-heart-{i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

            image.transform.SetParent(parentObj, false);
            
            var rt = image.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            
            rt.sizeDelta = new Vector2(80,80);

            rt.anchoredPosition = new Vector2(80 * i, 0f);

            var img = image.GetComponent<Image>();
            img.sprite = healthSprites[1];
            img.preserveAspect = true;
        }
    }

    void EndLevel()
    {
        var endLevelObj = gameObject.transform.GetChild(2).gameObject;
        
        endLevelObj.SetActive(true);
    }

    void PlayerDead()
    {
        var gameoverObj = gameObject.transform.GetChild(3).gameObject;
        
        gameoverObj.SetActive(true);
    }
}
