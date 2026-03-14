using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // DEBUG: Confirms the hitbox touched the player
            Debug.Log("<color=orange>Hitbox Triggered:</color> Contact with Player detected.");

            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(_damage);
            }
        }
    }
}