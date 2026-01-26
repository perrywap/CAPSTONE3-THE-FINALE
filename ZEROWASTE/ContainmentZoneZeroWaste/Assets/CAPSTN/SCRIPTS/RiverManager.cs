using UnityEngine;

public enum RiverState
{
    Dirty,
    Neutral,
    Clean
}

public class RiverManager : MonoBehaviour
{
    public static RiverManager Instance;

    [Header("River Settings")]
    [SerializeField] private int maxValue = 100;
    [SerializeField] private int currentValue = 50;

    [Header("State Thresholds")]
    [SerializeField] private int dirtyThreshold = 30;
    [SerializeField] private int cleanThreshold = 70;

    [Header("Enemy Buffs (Dirty Water)")]
    public float enemyHealthMultiplier = 1.3f;
    public float enemySpeedMultiplier = 1.2f;

    [Header("Tower Buffs (Clean Water)")]
    public float towerDamageMultiplier = 1.25f;
    public float towerFireRateMultiplier = 1.2f;

    public RiverState CurrentState { get; private set; }

    private void Awake()
    {
        Instance = this;
        UpdateState();
    }

    public void DamageRiver(int amount)
    {
        currentValue -= amount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        UpdateState();
    }

    public void CleanRiver(int amount)
    {
        currentValue += amount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        UpdateState();
    }

    public RiverState GetRiverState()
    {
        return CurrentState;
    }

    void UpdateState()
    {
        if (currentValue <= dirtyThreshold)
            CurrentState = RiverState.Dirty;
        else if (currentValue >= cleanThreshold)
            CurrentState = RiverState.Clean;
        else
            CurrentState = RiverState.Neutral;
    }

    public float GetEnemyHealthMultiplier()
    {
        if (CurrentState == RiverState.Dirty)
            return enemyHealthMultiplier;

        return 1f;
    }

    public float GetEnemySpeedMultiplier()
    {
        if (CurrentState == RiverState.Dirty)
            return enemySpeedMultiplier;

        return 1f;
    }

    public float GetTowerDamageMultiplier()
    {
        if (CurrentState == RiverState.Clean)
            return towerDamageMultiplier;

        return 1f;
    }

    public float GetTowerFireRateMultiplier()
    {
        if (CurrentState == RiverState.Clean)
            return towerFireRateMultiplier;

        return 1f;
    }
}
