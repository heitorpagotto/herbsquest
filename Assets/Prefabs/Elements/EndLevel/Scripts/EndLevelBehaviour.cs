using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelBehaviour : MonoBehaviour
{
    [SerializeField] private int currentLevel;
    [SerializeField] private string sceneToLoad = "Map";

    public event Action OnLevelEnd;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance?.PlaySfx("LevelGoal");
            AudioManager.Instance?.musicSource.Stop();
            
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
                FinshedLevels = new int[] { currentLevel },
                LastLevel = currentLevel,
                UnlockedLevels = new int[] { currentLevel, currentLevel + 1 },
                PlayerMaxHealth = maxHealth,
            };
        }
        else
        {
            data.FinshedLevels = data.FinshedLevels?.Concat(new int[] { currentLevel }).ToArray() ?? new int[] { currentLevel };
            data.LastLevel = currentLevel;
            data.UnlockedLevels = data.UnlockedLevels?.Concat(new int[] { currentLevel + 1 }).ToArray() ?? new int[] { currentLevel + 1 };
            data.PlayerMaxHealth = maxHealth;
        }
        
        GameSave.SaveGame(data);
    }

    IEnumerator PlaySoundAndLoadMap()
    {
        AudioManager.Instance?.PlaySfx("LevelClear");
        
        yield return new WaitForSeconds(4);

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
}
