using System.Collections;
using Enums;
using JetBrains.Annotations;
using UnityEngine;

public class PipeBehaviour : MonoBehaviour
{
    [SerializeField] [CanBeNull] private PipeBehaviour destination;

    // How fast the player moves into/out of the pipe
    [SerializeField] private float pipeSpeed = 3f;

    // Vertical distance inside the pipe before teleporting (just a bit beyond the pipe edge)
    [SerializeField] private float verticalOffsetInside = 0.2f;

    [SerializeField] private EPipeDirection pipeDirection = EPipeDirection.Up;

    public void Teleport(GameObject player)
    {
        if (destination == null) return;

        // 1) Grab references and disable player control/physics
        var playerController = player.GetComponent<PlayerController>();
        var capsuleCollider = player.GetComponent<Collider2D>();
        var rb = player.GetComponent<Rigidbody2D>();
        var animator = player.GetComponent<Animator>();

        playerController.enabled = false;
        capsuleCollider.enabled = false;
        animator.SetFloat("speed", 0f);
        animator.SetBool("jumping", false);
        animator.SetBool("falling", false);
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; // turn off physics completely

        // 2) Compute “top of this pipe” coordinates
        var entryPipeCollider = GetComponent<BoxCollider2D>();
        var entryTopY = entryPipeCollider.bounds.max.y;
        var pipeCenterX = entryPipeCollider.bounds.center.x;

        // 3) Compute “inside bottom” target: a little below the top (so player appears to go inside)
        var entryTargetY = entryTopY - (player.GetComponent<SpriteRenderer>().bounds.size.y / 2f) -
                           verticalOffsetInside;

        // 4) Start coroutine to descend, teleport, ascend
        StartCoroutine(DoPipeTransition(player.transform, pipeCenterX, entryTargetY, playerController, capsuleCollider,
            rb));
    }

    private IEnumerator DoPipeTransition(
        Transform playerT,
        float entryCenterX,
        float entryTargetY,
        PlayerController controller,
        Collider2D playerCollider2D,
        Rigidbody2D rb)
    {
        if (destination == null) yield return new WaitForSeconds(1f);

        // ** STEP A: Move the player horizontally and vertically down into the entry pipe **

        // Snap X to the pipe’s center
        var startPos = playerT.position;
        startPos.x = entryCenterX;
        playerT.position = startPos;

        // Now smoothly move Y from current Y down to entryTargetY
        while (playerT.position.y > entryTargetY)
        {
            playerT.position += Vector3.down * pipeSpeed * Time.deltaTime;
            yield return null;
        }

        // (Optional) play SFX/Pause briefly
        AudioManager.Instance?.PlaySfx("PipeEnter");
        yield return new WaitForSeconds(0.3f);

        // ** STEP B: Teleport to destination pipe’s “inside bottom” position **
        var exitPipeCollider = destination.GetComponent<BoxCollider2D>();
        var exitTopY = destination.pipeDirection == EPipeDirection.Up ? exitPipeCollider.bounds.max.y : exitPipeCollider.bounds.min.y;
        var exitCenterX = exitPipeCollider.bounds.center.x;

        // Compute “just inside” the exit pipe: slightly below the top edge
        var spriteHeight = playerT.GetComponent<SpriteRenderer>().bounds.size.y;
        var exitInsideY = exitPipeCollider.bounds.max.y - (spriteHeight / 2f) - verticalOffsetInside;

        // Place the player at that “inside bottom” point, but at the exit pipe X
        playerT.position = new Vector3(exitCenterX, exitInsideY, playerT.position.z);

        // (Optional) play SFX/Pause briefly
        AudioManager.Instance?.PlaySfx("PipeEnter");
        yield return new WaitForSeconds(0.3f);

        // ** STEP C: Rise up out of the exit pipe to its top edge **
        // Target Y is just above the exitTopY
        //var exitAboveY = exitTopY + (spriteHeight / 2f);
        var exitAboveY = destination.pipeDirection == EPipeDirection.Up ? exitTopY + (spriteHeight / 2f) : exitTopY - (spriteHeight / 2f);

        if (destination.pipeDirection == EPipeDirection.Up)
        {
            while (playerT.position.y < exitAboveY)
            {
                playerT.position += Vector3.up * (pipeSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            while (playerT.position.y > exitAboveY)
            {
                playerT.position += Vector3.down * (pipeSpeed * Time.deltaTime);
                yield return null;
            }
        }


        // ** STEP D: Restore physics, collisions, and control **
        rb.bodyType = RigidbodyType2D.Dynamic;
        playerCollider2D.enabled = true;
        controller.enabled = true;
    }
}