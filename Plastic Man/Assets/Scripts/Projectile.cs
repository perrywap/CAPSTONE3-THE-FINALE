using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject vfx;

    [Header("Movement")]
    public float speed = 10f;
    public float lifetime = 3f;

    [Header("Damage")]
    public float damage = 20f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            HandleImpact();
            return; // Exit so we don't trigger twice
        }

        BossEnemy boss = collision.GetComponent<BossEnemy>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            HandleImpact();
            return;
        }

        if (collision.CompareTag("Environment"))
        {
            HandleImpact();
        }
    }

    private void HandleImpact()
    {
        if (vfx != null)
        {
            Instantiate(vfx, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}