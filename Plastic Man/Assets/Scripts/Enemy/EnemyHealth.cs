using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 50f;
    private float _currentHealth;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSfx; // sound when hit
    [SerializeField] private AudioClip deathSfx;  // sound when dying
    [SerializeField] private float damageVolume = 0.5f;
    [SerializeField] private float deathVolume = 0.7f;

    void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        // Play damage sound
        if (damageSfx != null)
        {
            SfxManager.instance.PlaySFX(damageSfx, damageVolume);
        }

        Debug.Log($"<color=green>Enemy Hit!</color> Remaining HP: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Play death sound
        if (deathSfx != null)
        {
            SfxManager.instance.PlaySFX(deathSfx, deathVolume);
        }

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