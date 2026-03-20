using UnityEngine;

public class RoombaPuck : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_damage);
            }

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}