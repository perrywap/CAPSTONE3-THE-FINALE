using System.Collections;
using UnityEngine;

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
    [SerializeField] private float currentValue = 100;
    [SerializeField] private float maxValue = 100;


    [Header("River State Thresholds")]
    [SerializeField] private float dirtyThreshold = 30;
    [SerializeField] private float cleanThreshold = 70;


    [Header("Visuals")]
    [SerializeField] private SpriteRenderer riverRenderer;
    [SerializeField] private Gradient riverGradient;

    public RiverState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        StartCoroutine(RiverTest());
    }

    private void Update()
    {
        UpdateState();
        UpdateVisual();
    }

    public void DamageRiver(float amount)
    {
        currentValue -= amount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        UpdateState();
        UpdateVisual();
    }

    public void CleanRiver(float amount)
    {
        currentValue += amount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        UpdateState();
        UpdateVisual();
    }

    public RiverState GetRiverState()
    {
        return CurrentState;
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
        if (riverRenderer == null)
            return;

        float t = (float)currentValue / maxValue;
        riverRenderer.color = riverGradient.Evaluate(t);
    }

    private IEnumerator RiverTest()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            DamageRiver(0.5f);
        }
        
    }
}
