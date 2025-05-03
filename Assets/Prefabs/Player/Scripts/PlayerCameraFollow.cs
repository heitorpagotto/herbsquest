using System;
using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [SerializeField] private Vector3 offset;

    [SerializeField] private float damping;

    [SerializeField] private CameraBounds cameraBounds;  

    private Vector3 _velocity = Vector3.zero;

    private void FixedUpdate()
    {
        Vector3 targetPosition = playerTransform.position + offset;
        targetPosition.z = transform.position.z;

        if (targetPosition.x <= cameraBounds.LeftBound)
            targetPosition.x = cameraBounds.LeftBound;
        
        if (targetPosition.x >= cameraBounds.RightBound)
            targetPosition.x = cameraBounds.RightBound;
        
        if (targetPosition.y >= cameraBounds.TopBound)
            targetPosition.y = cameraBounds.TopBound;
        
        if (targetPosition.y <= cameraBounds.BottomBound)
            targetPosition.y = cameraBounds.BottomBound;

        // if (targetPosition.y <= CalcPercentage(playerTransform.position.y, transform.position.y))
        //     targetPosition.y = transform.position.y;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, damping);
    }

    private float CalcPercentage(float initial, float whole)
    {
        return Math.Abs(Math.Abs(initial) * 100 / Math.Abs(whole));
    }
}