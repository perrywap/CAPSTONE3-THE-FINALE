using UnityEngine;
using System.Collections.Generic;

public class GranadeExplosion : MonoBehaviour
{
    public float damage = 20f;
    public float lifetime = 0.3f; // match your 12-frame animation

    private HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>();

    void Start()
    {
        // Damage enemies already inside explosion
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f);

        foreach (var hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();

            if (enemyHealth != null && !hitEnemies.Contains(enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
                hitEnemies.Add(enemyHealth);
            }
        }

        // Destroy after animation duration
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

        if (enemyHealth != null && !hitEnemies.Contains(enemyHealth))
        {
            enemyHealth.TakeDamage(damage);
            hitEnemies.Add(enemyHealth);
        }
    }
}