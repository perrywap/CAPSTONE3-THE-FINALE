using UnityEngine;
using System.Collections.Generic;

public class GranadeExplosion : MonoBehaviour
{
    public float damage = 20f;
    public float lifetime = 0.3f;

    private HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>();
    private HashSet<BossEnemy> hitBosses = new HashSet<BossEnemy>();

    void Start()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f);

        foreach (var hit in hits)
        {
            // NORMAL ENEMIES
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                enemy.TakeDamage(damage);
                hitEnemies.Add(enemy);
            }

            // BOSS (from parent)
            BossEnemy boss = hit.GetComponentInParent<BossEnemy>();
            if (boss != null && !hitBosses.Contains(boss))
            {
                boss.TakeDamage(damage);
                hitBosses.Add(boss);
            }
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // NORMAL ENEMIES
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null && !hitEnemies.Contains(enemy))
        {
            enemy.TakeDamage(damage);
            hitEnemies.Add(enemy);
        }

        // BOSS
        BossEnemy boss = collision.GetComponentInParent<BossEnemy>();
        if (boss != null && !hitBosses.Contains(boss))
        {
            boss.TakeDamage(damage);
            hitBosses.Add(boss);
        }
    }
}