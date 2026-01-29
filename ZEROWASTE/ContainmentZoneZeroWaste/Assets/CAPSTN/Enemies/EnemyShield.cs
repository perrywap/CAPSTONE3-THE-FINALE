using UnityEngine;

public class EnemyShield : MonoBehaviour { 
    [Header("Shield Settings")]
    [SerializeField] private float maxShieldHp = 50f;

    private float currentShieldHp;
    private bool shieldBroken;

    void Awake()
    {
        currentShieldHp = maxShieldHp;
    }

    public bool AbsorbDamage(float damage)
    {
        if (shieldBroken)
            return false;

        currentShieldHp -= damage;

        if (currentShieldHp <= 0f)
        {
            BreakShield();
        }

        return true;
    }

    void BreakShield()
    {
        shieldBroken = true;

        gameObject.SetActive(false);
    }
}
