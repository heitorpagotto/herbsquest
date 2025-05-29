using System.Collections;
using UnityEngine;

public class PassThroughBehaviour : MonoBehaviour
{
    [SerializeField] private Transform playerPosition;
    
    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SetCorrectCollision();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StartCoroutine(CanEnableBoxCollider());
    }

    IEnumerator CanEnableBoxCollider()
    {
        yield return new WaitForEndOfFrame();
        
        _boxCollider.enabled = playerPosition.position.y > gameObject.transform.position.y;
    }

    void SetCorrectCollision()
    {
        var totalTiles = _spriteRenderer.size.x;

        var size = _boxCollider.size;
        size.x = totalTiles;

        _boxCollider.size = size;
    }
}
