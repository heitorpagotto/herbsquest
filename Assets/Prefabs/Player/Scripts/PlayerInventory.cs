using System;
using Enums;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField]
    private int MaxHealth = 2;
    
    [SerializeField]
    private int CurrentHealth = 2;

    [SerializeField]
    private int Coins = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(ECollectibles.Coin.ToString()))
        {
            other.GetComponent<CoinBehaviour>().Collect();
            Coins++;
        }
    }
}
