using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject vfx;

    [Header("Movement")]
    public float speed = 10f;
    public float lifetime = 3f;

    [Header("Damage")]
    public float damage;

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
        rb.linearVelocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyAI enemy = collision.GetComponent<EnemyAI>();

        if (enemy != null)
        {
            Instantiate(vfx, this.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        
    }
}