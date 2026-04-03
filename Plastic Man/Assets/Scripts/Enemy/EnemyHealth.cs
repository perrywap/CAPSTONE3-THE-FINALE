using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 50f;
    private float _currentHealth;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSfx;
    [SerializeField] private float damageVolume = 0.5f;

    public float CurrentHealth { get { return _currentHealth; } }
    public float MaxHealth { get { return _maxHealth; } }

    void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (_currentHealth <= 0) return; 

        _currentHealth -= damage;

        var hpBar = GetComponentInChildren<HpBarComponent>();
        if (hpBar != null) hpBar.OnDamaged();

        if (damageSfx != null)
        {
            SfxManager.instance.PlaySFX(damageSfx, damageVolume);
        }

        Debug.Log($"<color=green>Enemy Hit!</color> Remaining HP: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        EnemyAI slimeAI = GetComponent<EnemyAI>();
        RoombaAI roombaAI = GetComponent<RoombaAI>();

        if (slimeAI != null)
        {
            slimeAI.Die();
        }
        else if (roombaAI != null)
        {
            roombaAI.Die();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterEnemy(this.gameObject);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.UnregisterEnemy(this.gameObject);
    }
}