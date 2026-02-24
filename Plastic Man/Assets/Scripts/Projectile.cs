using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public float lifetime = 3f;

    [Header("Damage")]
    public int damage = 1;

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
        
        Destroy(gameObject);
    }
}