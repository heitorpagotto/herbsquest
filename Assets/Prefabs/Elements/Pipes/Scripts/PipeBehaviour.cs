using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class PipeBehaviour : MonoBehaviour
{
    [SerializeField]
    [CanBeNull]
    private PipeBehaviour destination;

    
    
    public void Teleport(GameObject player)
    {
        AudioManager.Instance.PlaySfx("PipeEnter");
        
        if (destination == null) return;
        
        var playerController = player.GetComponent<PlayerController>();
        var capsuleCollider = player.GetComponent<CapsuleCollider2D>();
        var rigidBody2D = player.GetComponent<Rigidbody2D>();
        playerController.enabled = false;

        var playerTransform = player.transform;

        //capsuleCollider.enabled = false;

        //rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
        //rigidBody2D.gravityScale = 0;

        // TODO: arrumar transform

        var pipeCollider = gameObject.GetComponent<BoxCollider2D>();
        pipeCollider.excludeLayers = -1;
        
        pipeCollider.enabled = false;

        // var scale = player.transform.localScale;
        // scale.y = 0.5f;
        // player.transform.localScale = scale;


        StartCoroutine(AwaitForTeleportation(playerTransform, capsuleCollider, rigidBody2D, playerController, pipeCollider));
    }

    IEnumerator AwaitForTeleportation(Transform playerPosition, CapsuleCollider2D capsuleCollider, Rigidbody2D rigidBody, PlayerController controller, BoxCollider2D pipeCollider)
    {
        // var initialPosition = playerPosition.position;
        //
        // var targetPositionAnim = new Vector3(initialPosition.x, initialPosition.y - 10f, initialPosition.z);
        // playerPosition.position = Vector3.MoveTowards(initialPosition, targetPositionAnim, 100f * Time.deltaTime);
        //
        // while (Vector3.Distance(playerPosition.position, targetPositionAnim) > 0.01f)
        // {
        //     playerPosition.position = Vector3.MoveTowards(initialPosition, targetPositionAnim, 10f * Time.deltaTime);
        //     yield return null;
        // }
        //
        yield return new WaitForSeconds(1);
        
        playerPosition.position = destination!.transform.position;

        var targetFinalPositionAnim = new Vector3(playerPosition.position.x, playerPosition.position.y + 100f);
        playerPosition.position = Vector3.MoveTowards(playerPosition.position, targetFinalPositionAnim, 100f * Time.deltaTime);
        
        yield return new WaitForSeconds(1);
        
        controller.enabled = true;
        capsuleCollider.enabled = true;
        rigidBody.gravityScale = 1;
        pipeCollider.enabled = true;
        pipeCollider.excludeLayers = 0;
        
        //rigidBody.bodyType = RigidbodyType2D.Dynamic;
    }
}
