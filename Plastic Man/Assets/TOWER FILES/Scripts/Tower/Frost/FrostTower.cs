using UnityEngine;

public class FrostTower : Tower
{
    [Header("Frost Settings")]
    [SerializeField] private GameObject frostballPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Animation")]
    [SerializeField] private FrostTowerAnimator animator;

    [Header("Visual References")]
    [SerializeField] private GameObject frostMageObject;

    private Vector3 frostMageOriginalScale;

    private void Start()
    {
        if (frostMageObject != null)
            frostMageOriginalScale = frostMageObject.transform.localScale;

        if (animator != null)
        {
            animator.OnCastFrameReached += () =>
            {
                if (currentTarget != null)
                {
                    Attack(currentTarget);
                    fireCooldown = 1f / fireRate;
                }
            };
        }
    }

    protected override void Update()
    {
        fireCooldown -= Time.deltaTime;
        RemoveNullTargets();

        if (currentTarget == null || Vector2.Distance(transform.position, currentTarget.transform.position) > range + 0.1f)
        {
            currentTarget = GetNearestTarget();
        }

        if (currentTarget != null)
        {
            animator.PlayAttack();
            FlipMage(currentTarget.transform.position.x);
        }
        else
        {
            animator.PlayIdle();
            ResetToIdleFacingLeft();
        }
    }

    public override void Attack(Unit target)
    {
        if (frostballPrefab != null && firePoint != null && target != null)
        {
            GameObject frostball = Instantiate(frostballPrefab, firePoint.position, Quaternion.identity);
            FrostProjectile projectile = frostball.GetComponent<FrostProjectile>();

            if (projectile != null)
            {
                projectile.SetTarget(target, damage);
            }
        }
    }

    private void FlipMage(float targetX)
    {
        if (frostMageObject == null) return;

        Vector3 scale = frostMageOriginalScale;
        bool shouldFlip = targetX > transform.position.x;

        scale.x = Mathf.Abs(scale.x) * (shouldFlip ? -1 : 1);
        frostMageObject.transform.localScale = scale;
    }

    private void ResetToIdleFacingLeft()
    {
        if (frostMageObject != null)
            frostMageObject.transform.localScale = frostMageOriginalScale;
    }
}
