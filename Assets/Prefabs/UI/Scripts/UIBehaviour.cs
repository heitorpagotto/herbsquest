using TMPro;
using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private PlayerInventory inventoryScript;
    [SerializeField] private Sprite[] healthSprites;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCoinUI();
    }

    private void OnEnable()
    {
        inventoryScript.OnCoinChange += UpdateCoinUI;
    }

    private void OnDisable()
    {
        inventoryScript.OnCoinChange -= UpdateCoinUI;
    }

    void UpdateCoinUI()
    {
        coinText.text = inventoryScript.Coins.ToString("D2");
    }

    void UpdateEmptyHealth()
    {
        // TODO: update health
        // for (int i = 0; i < inventoryScript.MaxHealth; i++)
        // {
        //     var image = new Image()
        // }
    }
}
