using UnityEngine;

public class MortarTower : Tower
{
    [Header("Mortar Settings")]
    [SerializeField] private GameObject mortarPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float minRange = 3f;
    [SerializeField] private float maxRange = 8f;

    [Header("Barrel Rotation")]
    [SerializeField] private Transform turretPivot;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float maxRotationAngle = 45f;

    [Header("Animation")]
    [SerializeField] private MortarTowerAnimator animator;

    private Quaternion currentTargetRotation;

    private void Start()
    {
        if (animator != null)
        {
            animator.OnFireFrameReached += () =>
            {
                Unit target = GetNearestValidTarget();
                if (target != null && fireCooldown <= 0f)
                {
                    Attack(target);
                    fireCooldown = 1f / fireRate;
                }
            };
        }
    }

    protected override void Update()
    {
        fireCooldown -= Time.deltaTime;
        RemoveNullTargets();

        currentTarget = GetNearestValidTarget();

        if (turretPivot != null)
        {
            if (currentTarget != null)
            {
                RotateTurretTowards(currentTarget);

                if (Quaternion.Angle(turretPivot.rotation, currentTargetRotation) == 0f && fireCooldown <= 0f)
                {
                    animator?.PlayAttack();
                }
            }
            else
            {
                RotateToIdle();
                animator?.StopAnimation();
            }
        }
    }

    public override void Attack(Unit target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (mortarPrefab != null && firePoint != null && target != null && distance >= minRange && distance <= maxRange)
        {
            GameObject mortar = Instantiate(mortarPrefab, firePoint.position, Quaternion.identity);
            MortarProjectile projectile = mortar.GetComponent<MortarProjectile>();

            if (projectile != null)
            {
                projectile.SetTarget(target, damage);
            }
        }
    }

    private Unit GetNearestValidTarget()
    {
        Unit closest = null;
        float shortestDist = Mathf.Infinity;

        foreach (Unit u in targetsInRange)
        {
            if (u == null || u.Type == UnitType.Flying) continue;

            float dist = Vector2.Distance(transform.position, u.transform.position);
            if (dist >= minRange && dist <= maxRange && dist < shortestDist)
            {
                shortestDist = dist;
                closest = u;
            }
        }

        return closest;
    }

    private void RotateTurretTowards(Unit target)
    {
        Vector3 directionToTarget = target.transform.position - turretPivot.position;

        float angleToTarget = Vector2.SignedAngle(Vector2.up, directionToTarget);

        float clampedAngle = Mathf.Clamp(angleToTarget, -maxRotationAngle, maxRotationAngle);

        currentTargetRotation = Quaternion.Euler(0f, 0f, clampedAngle);
        turretPivot.rotation = Quaternion.RotateTowards(
            turretPivot.rotation,
            currentTargetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void RotateToIdle()
    {
        Quaternion idleRotation = Quaternion.Euler(0f, 0f, 0f);
        turretPivot.rotation = Quaternion.RotateTowards(
            turretPivot.rotation,
            idleRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
}
