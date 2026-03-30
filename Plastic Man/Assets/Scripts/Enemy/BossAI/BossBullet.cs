using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 _direction;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _direction = (player.transform.position - transform.position).normalized;
        }
        Destroy(gameObject, 5f); 
    }

    void Update()
    {
        transform.Translate(_direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(10);
            Destroy(gameObject);
        }
    }
}