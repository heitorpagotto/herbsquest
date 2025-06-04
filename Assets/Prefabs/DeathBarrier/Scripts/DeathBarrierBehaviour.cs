using UnityEngine;

public class DeathBarrierBehaviour : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var inventory = other.GetComponent<PlayerInventory>();
            inventory.Die();
        }

        if (other.CompareTag("Enemy"))
        {
            Destroy(other);
        }
    }
}
