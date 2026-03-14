using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        // DEBUG: Shows exactly how much damage was taken and remaining health
        Debug.Log($"<color=red>Player Hit!</color> Damage taken: {damage}. Health remaining: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("<color=black><b>Player Died!</b></color>");
        // Optional: reload the scene or play a death animation
    }
}