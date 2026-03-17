using UnityEngine;
using UnityEngine.UI; // Required for interacting with UI elements

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    [Header("UI References")]
    [SerializeField] private Image _healthBarFill; 

    void Start()
    {
        _currentHealth = _maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        Debug.Log($"<color=red>Player Hit!</color> Damage taken: {damage}. Health remaining: {_currentHealth}");

        UpdateHealthBar();

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (_healthBarFill != null)
        {
            _healthBarFill.fillAmount = _currentHealth / _maxHealth;
        }
    }

    private void Die()
    {
        Debug.Log("<color=black><b>Player Died!</b></color>");
    }
}