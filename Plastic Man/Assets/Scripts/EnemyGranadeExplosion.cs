using UnityEngine;
using System.Collections.Generic;

public class EnemyGranadeExplosion : MonoBehaviour
{
    public float damage = 20f;
    public float lifetime = 0.3f;
    public float radius = 2f;

    private HashSet<PlayerHealth> hitPlayers = new HashSet<PlayerHealth>();

    void Start()
    {
        // Immediate AoE damage check (like explosion burst)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();

                if (playerHealth != null && !hitPlayers.Contains(playerHealth))
                {
                    playerHealth.TakeDamage(damage);
                    hitPlayers.Add(playerHealth);
                }
            }
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null && !hitPlayers.Contains(playerHealth))
            {
                playerHealth.TakeDamage(damage);
                hitPlayers.Add(playerHealth);
            }
        }
    }
}