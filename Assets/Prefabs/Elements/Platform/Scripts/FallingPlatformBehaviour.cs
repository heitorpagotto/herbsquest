using System;
using UnityEngine;
using System.Collections;

public class FallingPlatformBehaviour : MonoBehaviour
{
    [SerializeField]
    private float dropDelay = 1f;
    [SerializeField]
    private float resetDelay = 3f;
    [SerializeField] private float fallingDistance = 5f;
    [SerializeField] private float fallingSpeed = 5f;
    [SerializeField]
    private AnimationCurve returnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField]
    private float returnDuration = 0.5f; // How quickly to slide back

    private Vector3 _initialPos;
    private Collider2D _collider;
    private bool _hasDropped = false;

    public bool hasDropped => _hasDropped;
    
    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _initialPos = transform.position;
    }
    

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.collider.tag);
        if (!_hasDropped && collision.collider.CompareTag("Player"))
        {
            foreach (ContactPoint2D p in collision.contacts)
            {
                Debug.Log(p.normal.y);
                if (p.normal.y <= -0.5f)
                {
                    _hasDropped = true;
                    StartCoroutine(DropAndSlideBack());
                    break;
                }
            }
        }
    }
    
    
    private IEnumerator DropAndSlideBack()
    {
        // 1) Wait before disabling Effector
        yield return new WaitForSeconds(dropDelay);

        // 2) Disable the collider so the platform “falls through” (player drops with it).
        _collider.enabled = false;

        // Optionally, if you want it to actually move down:
        Vector3 fallTarget = _initialPos + Vector3.down * fallingDistance;
        float elapsed = 0f;

        // Slide down manually
        while (elapsed < (fallingDistance / fallingSpeed))
        {
            transform.position = Vector3.Lerp(_initialPos, fallTarget, elapsed * fallingSpeed / fallingDistance);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = fallTarget;

        // 3) Wait at bottom
        yield return new WaitForSeconds(resetDelay);

        // 4) Slide back up
        float timer = 0f;
        Vector3 startPos = transform.position;
        while (timer < returnDuration)
        {
            float t = returnCurve.Evaluate(timer / returnDuration);
            transform.position = Vector3.Lerp(startPos, _initialPos, t);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = _initialPos;

        // 5) Re-enable collider
        _collider.enabled = true;
        _hasDropped = false;
    }
}
