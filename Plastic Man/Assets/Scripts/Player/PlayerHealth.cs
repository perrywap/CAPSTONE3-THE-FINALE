using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Health Settings")]
    [SerializeField] private float _baseMaxHealth = 100f;
    private float _totalMaxHealth;
    private float _currentHealth;
    public bool isDead;

    [Header("UI References")]
    [SerializeField] private Image _healthBarFill;
    [SerializeField] private GameObject _gameOverPanel;

    [Header("Blink Settings")]
    [SerializeField] private int _blinkCount = 3;
    [SerializeField] private float _blinkSpeed = 0.1f;

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private bool _isBlinking = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;
        if (_gameOverPanel != null) _gameOverPanel.SetActive(false);

        UpdateMaxHealth();
        _currentHealth = _totalMaxHealth;
        UpdateHealthBar();
    }

    public void UpdateMaxHealth()
    {
        float bonus = 0;
        PlayerModule[] equippedModules = GetComponentsInChildren<PlayerModule>();

        foreach (var mod in equippedModules)
        {
            bonus += mod.BonusHealth;
        }

        _totalMaxHealth = _baseMaxHealth + bonus;
        _currentHealth = Mathf.Min(_currentHealth, _totalMaxHealth);
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _totalMaxHealth);
        UpdateHealthBar();

        if (!_isBlinking && _currentHealth > 0)
        {
            StartCoroutine(BlinkRed());
        }

        if (_currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
    }

    private IEnumerator BlinkRed()
    {
        _isBlinking = true;
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < _blinkCount; i++)
        {
            foreach (var spr in sprites) if (spr != null) spr.color = Color.red;
            yield return new WaitForSeconds(_blinkSpeed);
            foreach (var spr in sprites) if (spr != null) spr.color = _originalColor;
            yield return new WaitForSeconds(_blinkSpeed);
        }

        _isBlinking = false;
    }

    private void UpdateHealthBar()
    {
        if (_healthBarFill != null)
        {
            _healthBarFill.fillAmount = _currentHealth / _totalMaxHealth;
        }
    }

    private void Die()
    {
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        if (_spriteRenderer != null) _spriteRenderer.enabled = false;
        this.enabled = false;
    }
}