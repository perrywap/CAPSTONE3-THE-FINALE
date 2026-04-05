using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float travelTime = 0.6f;
    [SerializeField] private float arcHeight = 2f;

    [Header("VFX")]
    [SerializeField] private GameObject impactVFX;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private float timer = 0f;
    private bool hasTarget = false;
    private bool hasExploded = false;

    public void SetTarget(Vector3 target)
    {
        startPosition = transform.position;

        targetPosition = target;
        targetPosition.z = 0f;

        timer = 0f;
        hasTarget = true;
        hasExploded = false;
    }

    void Update()
    {
        if (!hasTarget || hasExploded) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / travelTime);

        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        Vector3 pos = Vector3.Lerp(startPosition, targetPosition, smoothT);

        float height = 4f * arcHeight * smoothT * (1f - smoothT);
        pos.y += height;

        transform.position = pos;

        transform.Rotate(0f, 0f, 720f * Time.deltaTime);

        if (t >= 1f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (impactVFX != null)
        {
            Vector3 spawnPos = new Vector3(targetPosition.x, targetPosition.y, -1f);
            Instantiate(impactVFX, spawnPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}