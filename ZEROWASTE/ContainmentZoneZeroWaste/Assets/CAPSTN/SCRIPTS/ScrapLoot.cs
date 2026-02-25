using Mono.Cecil;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrapLoot : Scraps
{
    [Header("Bounce Settings")]
    [SerializeField] private float travelTime = 0.35f;
    [SerializeField] private float bounceHeight = 0.5f;
    [SerializeField]
    private AnimationCurve heightCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Visuals")]
    [SerializeField] private Transform sprite;   // the visible sprite

    private Vector3 startPos;
    private Vector3 targetPos;
    private float timer;

    public void Init(Vector3 target, int rewardAmount)
    {
        startPos = transform.position;
        targetPos = target;
        amount = rewardAmount;

        // Random 2D rotation (Z axis only)
        float randomZ = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomZ);
    }
    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / travelTime;

        if (t >= 1f)
        {
            transform.position = targetPos;
            if (sprite != null) sprite.localPosition = Vector3.zero;
            enabled = false;
            return;
        }

        Vector3 groundPos = Vector3.Lerp(startPos, targetPos, t);
        float height = heightCurve.Evaluate(t) * bounceHeight;

        transform.position = groundPos;

        if (sprite != null)
            sprite.localPosition = Vector3.up * height;
    }
}
