using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class PipeBehaviour : MonoBehaviour
{
    [SerializeField]
    [CanBeNull]
    private Transform destination;

    public void Teleport(GameObject player)
    {
        if (destination == null) return;
        
        var playerController = player.GetComponent<PlayerController>();
        var capsuleCollider = player.GetComponent<CapsuleCollider2D>();
        var rigidBody2D = player.GetComponent<Rigidbody2D>();
        playerController.enabled = false;

        var playerTransform = player.transform;
        var initialPosition = playerTransform.position;

        capsuleCollider.enabled = false;

        //rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
        rigidBody2D.gravityScale = 0;

        // TODO: arrumar transform
        
        var targetPositionAnim = new Vector3(initialPosition.x, initialPosition.y - 100f, initialPosition.z);
        playerTransform.position = Vector3.MoveTowards(initialPosition, targetPositionAnim, 10f * Time.deltaTime);
        
        // while (Vector3.Distance(playerTransform.position, targetPositionAnim) > 0.01f)
        // {
        //     playerTransform.position = Vector3.MoveTowards(initialPosition, targetPositionAnim, 10f * Time.deltaTime);
        //     yield return null;
        // }

        StartCoroutine(AwaitForTeleportation(playerTransform, capsuleCollider, rigidBody2D, playerController));
    }

    IEnumerator AwaitForTeleportation(Transform playerPosition, CapsuleCollider2D capsuleCollider, Rigidbody2D rigidBody, PlayerController controller)
    {
        yield return new WaitForSeconds(1);
        
        playerPosition.position = destination!.position;

        var targetPositionAnim = new Vector3(playerPosition.position.x, playerPosition.position.y + 10f);
        playerPosition.position = Vector3.MoveTowards(playerPosition.position, targetPositionAnim, 10 * Time.deltaTime);
        
        yield return new WaitForSeconds(1);
        
        controller.enabled = true;
        capsuleCollider.enabled = true;
        rigidBody.gravityScale = 1;
        //rigidBody.bodyType = RigidbodyType2D.Dynamic;
    }
}
