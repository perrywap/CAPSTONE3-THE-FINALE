using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;

    [Header("UI References")]
    [SerializeField] private Image _healthBarFill;

    [Header("Blink Settings")]
    [SerializeField] private int _blinkCount = 3;
    [SerializeField] private float _blinkSpeed = 0.1f;

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private bool _isBlinking = false;

    void Start()
    {
        _currentHealth = _maxHealth;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;

        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        UpdateHealthBar();

        if (!_isBlinking)
        {
            StartCoroutine(BlinkRed());
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator BlinkRed()
    {
        _isBlinking = true;

        for (int i = 0; i < _blinkCount; i++)
        {
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(_blinkSpeed);
            _spriteRenderer.color = _originalColor;
            yield return new WaitForSeconds(_blinkSpeed);
        }

        _isBlinking = false;
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
        Debug.Log("Player Destroyed!");
        Destroy(gameObject);
    }
}