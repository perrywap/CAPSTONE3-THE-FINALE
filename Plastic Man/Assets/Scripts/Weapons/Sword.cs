using UnityEngine;

public class Sword : WeaponBase
{
    [SerializeField] private float range = 1.5f;
    [SerializeField] private LayerMask hitLayer;

    [Header("Audio")]
    [SerializeField] private AudioClip swingSfx;
    [SerializeField] private float swingVolume = 0.5f;

    protected override void Fire(Vector3 origin, Vector3 direction)
    {

        if (swingSfx != null)
        {
            SfxManager.instance.PlaySFX(swingSfx, swingVolume);
        }

        Vector3 hitPosition = origin + direction.normalized * range / 2f;
        float hitRadius = range / 2f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPosition, hitRadius, hitLayer);

        foreach (Collider2D hit in hits)
        {
            // 1. First, check if we hit a normal enemy
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Slashed Normal Enemy");
            }

            // 2. Next, check if we hit the Boss!
            BossEnemy boss = hit.GetComponentInParent<BossEnemy>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Debug.Log("Slashed the Boss!");
            }
        }

        Debug.DrawLine(origin, origin + direction.normalized * range, Color.red, 0.5f);
        Debug.DrawRay(hitPosition, Vector3.up * 0.01f, Color.green, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        if (transform == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range / 2f);
    }
}