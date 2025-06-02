using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelBehaviour : MonoBehaviour
{
    [SerializeField] private int currentLevel;

    public event Action OnLevelEnd;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var controller = other.GetComponent<PlayerController>();
            controller.enabled = false;
            
            var inventory = other.GetComponent<PlayerInventory>();

            Save(inventory.MaxHealth);
            
            OnLevelEnd?.Invoke();

            StartCoroutine(PlaySoundAndLoadMap());
        }
    }

    void Save(int maxHealth)
    {
        var data = GameSave.LoadGame();

        if (data == null)
        {
            data = new GameProgress()
            {
                CompletedLevels = new int[] { 1 },
                LastLevel = 1,
                UnlockedLevels = new int[] { 1, 2 },
                PlayerMaxHealth = maxHealth,
            };
        }
        else
        {
            data.CompletedLevels = new int[] { 1 };
            data.LastLevel = 1;
            data.UnlockedLevels = new int[] { 1, 2 };
            data.PlayerMaxHealth = maxHealth;
        }
        
        GameSave.SaveGame(data);
    }

    IEnumerator PlaySoundAndLoadMap()
    {
        // play sound
        
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("Map", LoadSceneMode.Single);
    }
}
