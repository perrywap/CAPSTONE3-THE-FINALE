using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 50f;
    private float _currentHealth;

    void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        Debug.Log($"<color=green>Enemy Hit!</color> Remaining HP: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        LootSpawner lootSpawner = Object.FindFirstObjectByType<LootSpawner>();

        if (lootSpawner != null)
        {
            lootSpawner.DropLoot(transform.position);
        }

        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.RemoveEnemyFromList(gameObject);
        }

        Destroy(gameObject);
    }
}