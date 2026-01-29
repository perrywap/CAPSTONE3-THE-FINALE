using UnityEngine;

public class PlasticCatcher : MonoBehaviour
{
    // Ensure the Collider on this object is set to "Is Trigger"
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if we hit an enemy
        EnemyBase enemy = collision.GetComponent<EnemyBase>();
        
        // If collider is on a child part, check parent
        if (enemy == null)
            enemy = collision.GetComponentInParent<EnemyBase>();

        if (enemy != null)
        {
            Debug.Log("[PlasticCatcher] Caught an enemy!");

            // 1. Kill the enemy immediately
            enemy.Die();

            // 2. Destroy this trap (one-time use)
            Destroy(gameObject);
        }
    }
}