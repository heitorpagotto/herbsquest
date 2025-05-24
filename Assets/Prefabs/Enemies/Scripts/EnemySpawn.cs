using System.Linq;
using Enums;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private int hitPoints = 1;
    [SerializeField] private EEnemyType enemyType;
    [SerializeField] private Sprite[] enemySprites;
    
    void Start()
    {
        SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy()
    {
        RenderSprite();
        SetEnemyCollision();
    }

    void RenderSprite()
    {
        var sprite = new GameObject("enemy-sprite");
        var spriteRenderer = sprite.AddComponent<SpriteRenderer>();

        Sprite validSprite = enemyType switch
        {
            EEnemyType.Flower => enemySprites.FirstOrDefault(x => x.name == "enemy1_0"),
            EEnemyType.Goomba => enemySprites.FirstOrDefault(x => x.name == "goomba"),
            _ => null
        };

        if (validSprite == null)
            return;
        
        spriteRenderer.sprite = validSprite;
        sprite.transform.SetParent(gameObject.transform);
        sprite.transform.localPosition = Vector3.zero;

        sprite.AddComponent<Rigidbody2D>();
        sprite.AddComponent<BoxCollider2D>();
    }
    
    void SetAnimationController() {}

    void SetEnemyCollision()
    {
        var collision = gameObject.AddComponent<BoxCollider2D>();
        collision.isTrigger = true;
        collision.tag = "death";
        collision.size = new Vector2(1, 0.75f);
        collision.offset = new Vector2(0, -0.25f);
        
        var hurtboxCollider = gameObject.AddComponent<BoxCollider2D>();
        hurtboxCollider.size = new Vector2(1, 0.2f);
        hurtboxCollider.isTrigger = true;
        hurtboxCollider.offset = new Vector2(0, 0.25f);
        hurtboxCollider.tag = "kill";
    }
}
