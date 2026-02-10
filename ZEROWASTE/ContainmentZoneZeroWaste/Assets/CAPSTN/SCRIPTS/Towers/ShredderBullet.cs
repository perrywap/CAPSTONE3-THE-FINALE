using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShredderBullet : Bullet
{
    private Vector2 direction;
    public float rotationSpeed = 2f;
    public void Init(Vector2 fireDirection, float dmg)
    {
        direction = fireDirection.normalized;
        damage = dmg;
    }

    private void ShredderRotation()
    {
        float rotationSpeed = -90f;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime * rotationSpeed);
    }

    public override void BulletMovement()
    {
        rb.linearVelocity = direction * bulletSpeed;
        ShredderRotation();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            HpBarComponent hpBar = collision.gameObject.GetComponentInChildren<HpBarComponent>(true);

            if (hpBar != null)
            {
                hpBar.TakeDamage(damage);
            }
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
