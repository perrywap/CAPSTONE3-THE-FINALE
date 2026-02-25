using UnityEngine;

public class ArcherTower : Tower
{
    [Header("Archer Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Visual References")]
    [SerializeField] private ArcherIdleAnimator idleAnimator;
    [SerializeField] private GameObject bowObject;
    [SerializeField] private GameObject archerSpriteObject;
    [SerializeField] private Transform bowPivot;
    [SerializeField] private float bowRotationOffset = 0f;

    [Header("Bow Rotation")]
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float angleThreshold = 2f;

    private Vector3 archerOriginalScale;
    private Vector3 bowOriginalScale;

    private Quaternion desiredRotation;
    private bool isRotatingToTarget = false;

    private void Start()
    {
        if (archerSpriteObject != null)
            archerOriginalScale = archerSpriteObject.transform.localScale;

        if (bowPivot != null)
            bowOriginalScale = bowPivot.localScale;
    }

    protected override void Update()
    {
        base.Update();

        Unit target = GetNearestTarget();

        if (target != null)
        {
            idleAnimator.StopIdle();
            bowObject.SetActive(true);
            RotateBowTowardsSmooth(target);

            if (!isRotatingToTarget && fireCooldown <= 0f)
            {
                Attack(target);
                fireCooldown = 1f / fireRate;
            }
        }
        else
        {
            idleAnimator.StartIdle();
            bowObject.SetActive(false);
            ResetToIdleFacingLeft();
        }
    }

    public override void Attack(Unit target)
    {
        if (arrowPrefab != null && firePoint != null && target != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = arrow.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.SetTarget(target, damage);
            }
        }
    }

    private void RotateBowTowardsSmooth(Unit target)
    {
        if (target == null || firePoint == null || bowPivot == null) return;

        Vector3 direction = target.transform.position - firePoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f + bowRotationOffset;
        desiredRotation = Quaternion.Euler(0f, 0f, angle);

        bowPivot.rotation = Quaternion.RotateTowards(bowPivot.rotation, desiredRotation, rotationSpeed * Time.deltaTime);

        FlipVisuals(direction.x);

        float angleDifference = Quaternion.Angle(bowPivot.rotation, desiredRotation);
        isRotatingToTarget = angleDifference > angleThreshold;
    }

    private void FlipVisuals(float xDirection)
    {
        const float flipThreshold = 0.2f;

        if (Mathf.Abs(xDirection) < flipThreshold)
            return;

        bool shouldFlip = xDirection > 0;

        if (archerSpriteObject != null)
        {
            Vector3 scale = archerOriginalScale;
            scale.x = Mathf.Abs(scale.x) * (shouldFlip ? -1 : 1);
            archerSpriteObject.transform.localScale = scale;
        }

        if (bowPivot != null)
        {
            Vector3 scale = bowOriginalScale;
            scale.x = Mathf.Abs(scale.x) * (shouldFlip ? -1 : 1);
            bowPivot.localScale = scale;
            bowPivot.rotation = desiredRotation;
        }
    }

    private void ResetToIdleFacingLeft()
    {
        if (archerSpriteObject != null)
            archerSpriteObject.transform.localScale = archerOriginalScale;

        if (bowPivot != null)
        {
            bowPivot.localScale = bowOriginalScale;
            bowPivot.rotation = Quaternion.identity;
        }

        isRotatingToTarget = false;
    }
}
