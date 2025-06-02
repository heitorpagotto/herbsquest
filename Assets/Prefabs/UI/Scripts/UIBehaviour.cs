using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private PlayerInventory inventoryScript;
    [SerializeField] private Sprite[] healthSprites;
    [SerializeField] private EndLevelBehaviour endLevelBehaviour;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCoinUI();
        UpdateEmptyHealth();
        UpdateFilledHealth();
    }

    private void OnEnable()
    {
        inventoryScript.OnCoinChange += UpdateCoinUI;
        inventoryScript.OnMaxHealthChange += UpdateEmptyHealth;
        inventoryScript.OnCurrentHealthChange += UpdateFilledHealth;
        endLevelBehaviour.OnLevelEnd += EndLevel;
    }

    private void OnDisable()
    {
        inventoryScript.OnCoinChange -= UpdateCoinUI;
        inventoryScript.OnMaxHealthChange -= UpdateEmptyHealth;
        inventoryScript.OnCurrentHealthChange -= UpdateFilledHealth;
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
}
