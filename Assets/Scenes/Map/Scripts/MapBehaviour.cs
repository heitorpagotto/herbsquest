using System;
using System.Linq;
using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapBehaviour : MonoBehaviour
{
    [SerializeField] private LevelNode[] levels;
    [SerializeField] private Sprite[] levelSprites;
    [SerializeField] private GameObject playerCursor;

    private int _currentSelectedLevel;
    private GameProgress _data;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance?.PlayMusic("MapTheme", 0f);
        _data = GetGameData();
        _currentSelectedLevel = _data.LastLevel;
        DrawLevels();
    }


    GameProgress GetGameData()
    {
        var data = GameSave.LoadGame();

        if (data == null)
        {
            data = new GameProgress()
            {
                UnlockedLevels = new[] { 1 },
                PlayerMaxHealth = 2,
                LastLevel = 1
            };
            GameSave.SaveGame(data);
        }

        return data;
    }

    void DrawLevels()
    {
        foreach (var level in levels)
        {
            level.IsUnlocked = _data.UnlockedLevels.Any(x => x == level.Number);
            level.IsBeaten = _data.FinshedLevels?.Any(x => x == level.Number) ?? false;
            level.IsCompleted = _data.CompletedLevels?.Any(x => x == level.Number) ?? false;

            GameObject levelIcon = new GameObject($"level-{level.Number}", typeof(SpriteRenderer));

            levelIcon.transform.position = level.Coordinates;

            var sprite = levelIcon.GetComponent<SpriteRenderer>();
            sprite.sprite = levelSprites[SetLevelSprite(level)];
            sprite.sortingOrder = 1;

            levelIcon.transform.SetParent(gameObject.transform);

            if (level.Number == _currentSelectedLevel)
                SetCursorPosition(level.Coordinates, false);
        }
    }

    void DrawLines()
    {
    }

    void SetCursorPosition(Vector2 position, bool smooth)
    {
        if (!smooth)
            playerCursor.transform.position = position;
    }

    void MoveCursor()
    {
        var currentLevelNeighbors = levels.FirstOrDefault(x => x.Number == _currentSelectedLevel)?.Neighbors;
        int? bestMatch = null;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        switch (horizontal)
        {
            case 1:
                bestMatch = currentLevelNeighbors?.FirstOrDefault(x => x.Direction == EDirection.Right)?.LevelNumber;
                break;
            case -1:
                bestMatch = currentLevelNeighbors?.FirstOrDefault(x => x.Direction == EDirection.Left)?.LevelNumber;
                break;
        }

        switch (vertical)
        {
            case 1:
                bestMatch = currentLevelNeighbors?.FirstOrDefault(x => x.Direction == EDirection.Up)?.LevelNumber;
                break;
            case -1:
                bestMatch = currentLevelNeighbors?.FirstOrDefault(x => x.Direction == EDirection.Down)?.LevelNumber;
                break;
        }

        if (bestMatch == null) return;

        var level = levels.FirstOrDefault(x => x.Number == bestMatch);


        if (level is not { IsUnlocked: true }) return;

        _currentSelectedLevel = bestMatch.Value;
        SetCursorPosition(level.Coordinates, false);
    }

    int SetLevelSprite(LevelNode level)
    {
        var spriteIndex = 0;
        
        if (level.IsUnlocked)
            spriteIndex = 1;

        if (level.IsBeaten)
            spriteIndex = 2;

        if (level.IsCompleted)
            spriteIndex = 3;

        return spriteIndex;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCursor();

        if (Input.GetButtonDown("Jump"))
            EnterLevel();
    }

    void EnterLevel()
    {
        var level = levels.FirstOrDefault(x => x.Number == _currentSelectedLevel);
        
        AudioManager.Instance.PlaySfx("LevelEnter");

        if (level is { IsUnlocked: true })
        {
            AudioManager.Instance.musicSource.Stop();
            SceneManager.LoadScene(level.SceneName, LoadSceneMode.Single);
        }
    }
}

[Serializable]
public class LevelNode
{
    public int Number;
    public string Name;
    public Vector2 Coordinates;
    public string SceneName;
    public bool IsUnlocked;
    public bool IsBeaten;
    public bool IsCompleted;
    public LevelNeighbor[] Neighbors;
}

[Serializable]
public class LevelNeighbor
{
    public EDirection Direction;
    public int LevelNumber;
}