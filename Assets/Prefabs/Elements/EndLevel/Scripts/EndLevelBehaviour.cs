using System;
using UnityEngine;

public class EndLevelBehaviour : MonoBehaviour
{
    [SerializeField] private int currentLevel;

    public event Action OnLevelEnd;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnLevelEnd?.Invoke();
        }
    }
}
