using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlatformBehaviour : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float fallingCooldown;
    [SerializeField] private float respawningCooldown;
    [SerializeField] private EPlatformType type;
    
    private SpriteRenderer _spriteRenderer;
    private bool _isFalling = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SetCorrectSprite();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.gameObject.tag);
        if (!_isFalling && other.gameObject.CompareTag("Player") && type == EPlatformType.Falling)
        {
            StartCoroutine(HandlePlatformFalling());
        }
    }

    IEnumerator HandlePlatformFalling()
    {
        _isFalling = true;
        
        var initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        var initialTarget = new Vector3(transform.position.x, transform.position.y - 5, transform.position.z);
        
        transform.position = Vector3.MoveTowards(transform.position, initialTarget, 5 * Time.deltaTime);
        
        yield return new WaitForSeconds(fallingCooldown);
        
        var rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.gravityScale = 10f;
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        
        
        // TODO: revisar
        yield return new WaitForSeconds(respawningCooldown);
        
        rigidBody.gravityScale = 0f;
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        transform.position = Vector3.MoveTowards(transform.position, initialPosition, 5 * Time.deltaTime);
        

        _isFalling = false;
    }

    void SetCorrectSprite()
    {
        Sprite sprite;
        
        switch (type)
        {
            case EPlatformType.Falling:
                sprite = sprites[1];
                _spriteRenderer.sprite = sprite;
                break;
            case EPlatformType.Safe:
                sprite = sprites[0];
                _spriteRenderer.sprite = sprite;

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
