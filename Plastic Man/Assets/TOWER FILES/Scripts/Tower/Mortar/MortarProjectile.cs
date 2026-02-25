using System.Collections;
using UnityEngine;

public class MortarProjectile : Projectile
{
    [Header("AOE Settings")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float arcHeight = 2f;
    [SerializeField] private GameObject explosionVFXPrefab;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float timeToTarget = 1f;
    private float elapsedTime = 0f;

    public override void SetTarget(Unit newTarget, float dmg)
    {
        base.SetTarget(newTarget, dmg);
        startPos = transform.position;
        targetPos = newTarget.transform.position;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / timeToTarget;

        if (t >= 1f)
        {
            Explode();
            return;
        }

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
        currentPos.y += arcHeight * 4 * t * (1 - t);
        transform.position = currentPos;
    }

    private void Explode()
    {
        if (explosionVFXPrefab != null)
        {
            Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            Unit enemy = hit.GetComponent<Unit>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                ApplyKnockback(enemy);
            }
        }

        GetComponent<SpriteRenderer>().enabled = false;
        enabled = false;
        Destroy(gameObject);
    }

    private void ApplyKnockback(Unit unit)
    {
        float knockbackStrength = 2f;
        float knockbackDuration = 0.25f;

        Vector2 knockbackDir = (unit.transform.position - transform.position).normalized;

        unit.StartCoroutine(Knockback(unit.transform, knockbackDir, knockbackStrength, knockbackDuration));
    }

    private IEnumerator Knockback(Transform targetTransform, Vector2 direction, float strength, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            targetTransform.position += (Vector3)(direction * strength * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}