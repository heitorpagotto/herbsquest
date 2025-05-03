using System;
using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    [SerializeField] private Vector3 offset;

    [SerializeField] private float damping;

    private Vector3 _velocity = Vector3.zero;

    private void FixedUpdate()
    {
        Vector3 targetPosition = playerTransform.position + offset;
        targetPosition.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, damping);
    }
}