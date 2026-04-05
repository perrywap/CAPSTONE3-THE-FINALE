using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _dashIcon;

    [Header("Visual Settings")]
    [Range(0, 1)]
    [SerializeField] private float _readyOpacity = 1.0f;     // Full brightness
    [Range(0, 1)]
    [SerializeField] private float _rechargeOpacity = 0.3f;  // Dimmed brightness

    private float _cooldownTimer = 0f;
    private float _maxCooldown = 1f;
    private bool _isOnCooldown = false;

    private void Start()
    {
        // Start state: Ready and Bright
        if (_dashIcon != null)
        {
            SetAlpha(_readyOpacity);
        }
    }

    private void Update()
    {
        if (_isOnCooldown)
        {
            _cooldownTimer -= Time.deltaTime;

            // Optional: If you still want the "Clock Wipe" effect while dimmed,
            // keep this line. If you ONLY want the dimming, delete this line.
            if (_dashIcon != null) _dashIcon.fillAmount = 1 - (_cooldownTimer / _maxCooldown);

            if (_cooldownTimer <= 0f)
            {
                _isOnCooldown = false;
                // Back to full brightness!
                SetAlpha(_readyOpacity);
                if (_dashIcon != null) _dashIcon.fillAmount = 1f;
            }
        }
    }

    public void StartCooldown(float cooldownDuration)
    {
        _maxCooldown = cooldownDuration;
        _cooldownTimer = cooldownDuration;
        _isOnCooldown = true;

        // Immediately dim the icon
        SetAlpha(_rechargeOpacity);
    }

    private void SetAlpha(float alpha)
    {
        if (_dashIcon != null)
        {
            Color c = _dashIcon.color;
            c.a = alpha;
            _dashIcon.color = c;
        }
    }
}