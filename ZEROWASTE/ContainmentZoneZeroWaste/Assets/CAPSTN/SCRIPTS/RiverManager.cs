//using UnityEngine;

//public enum RiverState
//{
//    Polluted,
//    Normal,
//    Clean
//}

//public class RiverManager : MonoBehaviour
//{
//    public static RiverManager Instance { get; private set; }

//    [Header("River Settings")]
//    [Range(0f, 100f)]
//    [SerializeField] private float currentValue = 100f;
//    [SerializeField] private float maxValue = 100f;
//    [SerializeField] private float regenRate = 2f;

//    [Header("River State Thresholds")]
//    [SerializeField] private float dirtyThreshold = 30f;
//    [SerializeField] private float cleanThreshold = 70f;

//    [Header("Visuals")]
//    [SerializeField] private SpriteRenderer riverRenderer;
//    [SerializeField] private Gradient riverGradient;

//    public RiverState CurrentState { get; private set; }

//    private void Awake()
//    {
//        if (Instance == null)
//            Instance = this;
//        else
//            Destroy(gameObject);
//    }

//    private void Update()
//    {
//        HandleRiverFlow();
//        UpdateState();
//        UpdateVisual();
//    }

//    private void HandleRiverFlow()
//    {
//        float totalDamage = 0f;

//        foreach (GameObject enemy in GameManager.Instance.enemies)
//        {
//            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();

//            if (enemyBase != null)
//                totalDamage += enemyBase.RiverDamagePerSecond * Time.deltaTime;
//        }

//        if (totalDamage > 0)
//            DamageRiver(totalDamage);
//        else
//            CleanRiver(regenRate * Time.deltaTime);
//    }

//    public void DamageRiver(float amount)
//    {
//        currentValue -= amount;
//        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
//    }

//    public void CleanRiver(float amount)
//    {
//        currentValue += amount;
//        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
//    }

//    private void UpdateState()
//    {
//        if (currentValue <= dirtyThreshold)
//            CurrentState = RiverState.Polluted;
//        else if (currentValue >= cleanThreshold)
//            CurrentState = RiverState.Clean;
//        else
//            CurrentState = RiverState.Normal;
//    }

//    private void UpdateVisual()
//    {
//        if (riverRenderer == null)
//            return;

//        float t = currentValue / maxValue;
//        riverRenderer.color = riverGradient.Evaluate(t);
//    }

//    public RiverState GetRiverState()
//    {
//        return CurrentState;
//    }
//}

using UnityEngine;
using UnityEngine.Tilemaps;

public enum RiverState
{
    Polluted,
    Normal,
    Clean
}

public class RiverManager : MonoBehaviour
{
    public static RiverManager Instance { get; private set; }

    [Header("River Settings")]
    [Range(0f, 100f)]
    [SerializeField] private float currentValue = 100f;
    [SerializeField] private float maxValue = 100f;
    [SerializeField] private float regenRate = 2f;

    [Header("River State Thresholds")]
    [SerializeField] private float dirtyThreshold = 30f;
    [SerializeField] private float cleanThreshold = 70f;

    [Header("Visuals")]
    [SerializeField] private Tilemap riverTilemap;
    [SerializeField] private Gradient riverGradient;

    public RiverState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        HandleRiverFlow();
        UpdateState();
        UpdateVisual();
    }

    private void HandleRiverFlow()
    {
        float totalDamage = 0f;

        foreach (GameObject enemy in GameManager.Instance.enemies)
        {
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();

            if (enemyBase != null)
                totalDamage += enemyBase.RiverDamagePerSecond * Time.deltaTime;
        }

        if (totalDamage > 0)
            DamageRiver(totalDamage);
        else
            CleanRiver(regenRate * Time.deltaTime);
    }

    public void DamageRiver(float amount)
    {
        currentValue -= amount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
    }

    public void CleanRiver(float amount)
    {
        currentValue += amount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
    }

    private void UpdateState()
    {
        if (currentValue <= dirtyThreshold)
            CurrentState = RiverState.Polluted;
        else if (currentValue >= cleanThreshold)
            CurrentState = RiverState.Clean;
        else
            CurrentState = RiverState.Normal;
    }

    private void UpdateVisual()
    {
        if (riverTilemap == null)
            return;

        float t = currentValue / maxValue;
        riverTilemap.color = riverGradient.Evaluate(t);
    }

    public RiverState GetRiverState()
    {
        return CurrentState;
    }
}
