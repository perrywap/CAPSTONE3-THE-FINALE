using System.Collections;
using UnityEngine;

public class FireballProjectile : Projectile
{
    [Header("Burn Settings")]
    [SerializeField] private float burnDamagePerSecond = 1f;
    [SerializeField] private int burnDuration = 3;
    [SerializeField] private GameObject burnVFXPrefab;

    [Header("Animation Settings")]
    [SerializeField] private Sprite[] animationFrames;
    [SerializeField] private float animationSpeed = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Unit target;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animationFrames.Length > 0)
        {
            StartCoroutine(PlayAnimation());
        }
    }

    public override void SetTarget(Unit newTarget, float dmg)
    {
        base.SetTarget(newTarget, dmg);
        target = newTarget;
    }

    private void Update()
    {
        if (!initialized || target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);

        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
        {
            target.TakeDamage(damage);
            StartCoroutine(ApplyBurn(target));

            spriteRenderer.enabled = false;
            enabled = false;
        }
    }

    private IEnumerator ApplyBurn(Unit enemy)
    {
        GameObject burnVFX = null;

        if (burnVFXPrefab != null && enemy != null)
        {
            burnVFX = Instantiate(burnVFXPrefab, enemy.transform.position, Quaternion.identity, enemy.transform);
        }

        for (int i = 0; i < burnDuration; i++)
        {
            yield return new WaitForSeconds(1f);

            if (enemy != null)
            {
                enemy.TakeDamage(burnDamagePerSecond);
            }
        }

        if (burnVFX != null)
        {
            Destroy(burnVFX);
        }

        Destroy(gameObject);
    }

    private IEnumerator PlayAnimation()
    {
        int index = 0;

        while (true)
        {
            if (spriteRenderer != null && animationFrames.Length > 0)
            {
                spriteRenderer.sprite = animationFrames[index];
                index = (index + 1) % animationFrames.Length;
            }

            yield return new WaitForSeconds(animationSpeed);
        }
    }
}
