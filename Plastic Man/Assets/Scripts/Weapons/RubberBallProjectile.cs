using UnityEngine;

public class RubberBallProjectile : Projectile2D
{
    [Header("Rubber Ball Settings")]
    [SerializeField] private int maxBounces = 5;

    private int bounceCount = 0;
    private Vector2 currentDirection;

    public override void Launch(Vector2 direction)
    {
        currentDirection = direction.normalized;
        rb.linearVelocity = currentDirection * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyHealth enemy = collision.collider.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            HandleImpact();
            return;
        }

        BossEnemy boss = collision.collider.GetComponentInParent<BossEnemy>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            HandleImpact();
            return;
        }

        if (collision.collider.CompareTag("Environment"))
        {
            ContactPoint2D contact = collision.GetContact(0);
            currentDirection = Vector2.Reflect(currentDirection, contact.normal).normalized;
            rb.linearVelocity = currentDirection * speed;

            bounceCount++;

            if (bounceCount >= maxBounces)
            {
                HandleImpact();
            }
        }
    }
}