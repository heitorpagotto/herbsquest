using System;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 2;
    
    [SerializeField]
    private int currentHealth = 2;

    [SerializeField]
    private int coins = 0;
    
    public event Action OnCoinChange;
    public event Action OnMaxHealthChange;
    public event Action OnCurrentHealthChange;

    public int Coins
    {
        get => coins;
        set
        {
            if (coins == value) return;
            coins = value;
            OnCoinChange?.Invoke();
        }
    }
    
    public int MaxHealth
    {
        get => maxHealth;
        set
        {
            if (maxHealth == value) return;
            maxHealth = value;
            OnMaxHealthChange?.Invoke();
        }
    }
    
    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            if (currentHealth == value) return;
            currentHealth = value;
            OnCurrentHealthChange?.Invoke();
        }
    }
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ECollectibles.Coin.ToString()))
        {
            other.GetComponent<CoinBehaviour>().Collect();
            Coins++;

            if (Coins > 99)
                Coins = 0;
        }
    }
    
    // This fires when you tweak the serialized fields in the Inspector at runtime:
    private void OnValidate()
    {
        OnCoinChange?.Invoke();
        OnMaxHealthChange?.Invoke();
        OnCurrentHealthChange?.Invoke();
    }
}
