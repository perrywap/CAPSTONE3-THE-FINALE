using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    
    private float damage; 

    private Transform target;

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public void SetDamage(float _damage)
    {
        damage = _damage;
    }

    private void FixedUpdate()
    {
        if (!target) 
            return;

        Vector2 direction = (target.position - transform.position).normalized;

        rb.linearVelocity = direction * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
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

        Destroy(gameObject);
    }
}