using System;
using Enums;
using Unity.VisualScripting;
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

            if (Coins > 50)
            {
                Coins = 0;
                Heal();
            }
        }

        if (other.CompareTag(ECollectibles.HeartUpgrade.ToString()))
        {
            other.GetComponent<HeartUpgradeBehaviour>().Collect();
            MaxHealth++;
            CurrentHealth = MaxHealth;
        }
        
        // if (other.CompareTag(ECollectibles.Heart.ToString()))
        // {
        //     CurrentHealth++;
        //
        //     if (CurrentHealth > MaxHealth)
        //         CurrentHealth = MaxHealth;
        // }
    }

    private void Heal()
    {
        CurrentHealth++;

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
    }

    public void TakeDamage()
    {
        CurrentHealth--;

        if (CurrentHealth == 0)
        {
            // Die();
        }
    }
    
    // This fires when you tweak the serialized fields in the Inspector at runtime:
    private void OnValidate()
    {
        OnCoinChange?.Invoke();
        
        if (!Application.isPlaying)
            return;
        
        OnMaxHealthChange?.Invoke();
        OnCurrentHealthChange?.Invoke();
    }
}
