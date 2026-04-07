using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    public bool isDead;

    [Header("UI References")]
    [SerializeField] private Image _healthBarFill;
    [SerializeField] private GameObject _gameOverPanel; 

    [Header("Blink Settings")]
    [SerializeField] private int _blinkCount = 3;
    [SerializeField] private float _blinkSpeed = 0.1f;

    [SerializeField] private AudioClip damagedSfx;

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private bool _isBlinking = false;

    public float MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _currentHealth = _maxHealth;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;

        if (_gameOverPanel != null) _gameOverPanel.SetActive(false);

        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        SfxManager.instance.PlaySFX(damagedSfx, 0.5f);
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        UpdateHealthBar();

        if (!_isBlinking && _currentHealth > 0)
        {
            StartCoroutine(BlinkRed());
        }

        if (_currentHealth <= 0)
        {
            isDead = true;
        }
    }

    private IEnumerator BlinkRed()
    {
        _isBlinking = true;

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < _blinkCount; i++)
        {
            foreach (var spr in sprites)
            {
                if(spr != null)
                    spr.color = Color.red; 
            }

            yield return new WaitForSeconds(_blinkSpeed);

            foreach (var spr in sprites)
            {
                if (spr != null)
                    spr.color = _originalColor;
            }

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

        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        _spriteRenderer.enabled = false;
        this.enabled = false;
    }
}