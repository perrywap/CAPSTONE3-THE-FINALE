using System.Collections.Generic;
using UnityEngine;

public class ShredderProjectile : Projectile2D
{
    [Header("Shredder Settings")]
    public float spinSpeed = 720f;

    private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

    void Update()
    {
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemy = collision.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            if (!hitTargets.Contains(collision))
            {
                hitTargets.Add(collision);
                enemy.TakeDamage(damage);
            }
            return;
        }

        BossEnemy boss = collision.GetComponentInParent<BossEnemy>();
        if (boss != null)
        {
            if (!hitTargets.Contains(collision))
            {
                hitTargets.Add(collision);
                boss.TakeDamage(damage);
            }
            return;
        }

        if (collision.CompareTag("Environment"))
        {
            HandleImpact();
        }
    }
}