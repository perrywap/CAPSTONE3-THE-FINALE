using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AntiAirTower : Tower
{
    [Header("Anti-Air Settings")]
    [SerializeField] private float lightningWidth = 0.1f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Animator animator;

    [Header("VFX")]
    [SerializeField] private GameObject hitVFXPrefab;

    private LineRenderer lineRenderer;
    private float tickTimer = 0f;
    private float flickerIntensity = 0.1f;
    private bool canShoot = false;
    private GameObject activeVFX;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        lineRenderer.startWidth = lightningWidth;
        lineRenderer.endWidth = lightningWidth;

        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected override void Update()
    {
        fireCooldown -= Time.deltaTime;
        RemoveNullTargets();

        if (currentTarget == null ||
            Vector2.Distance(transform.position, currentTarget.transform.position) > range + 0.1f ||
            currentTarget.Type != UnitType.Flying)
        {
            currentTarget = GetNearestFlyingTarget();
        }

        bool hasTarget = currentTarget != null;
        animator.SetBool("HasTarget", hasTarget);

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Start"))
        {
            if (boxCollider != null) boxCollider.enabled = false;
        }
        else
        {
            if (boxCollider != null) boxCollider.enabled = true;
        }

        if (hasTarget && canShoot)
        {
            lineRenderer.enabled = true;

            Vector3 jitter = Random.insideUnitSphere * flickerIntensity;
            lineRenderer.SetPosition(0, firePoint.position + jitter);
            lineRenderer.SetPosition(1, currentTarget.transform.position + jitter);

            if (activeVFX == null && hitVFXPrefab != null)
            {
                activeVFX = Instantiate(hitVFXPrefab, currentTarget.transform);
                activeVFX.transform.localPosition = Vector3.zero;
            }

            tickTimer += Time.deltaTime;
            if (tickTimer >= 1f / fireRate)
            {
                currentTarget.TakeDamage(damage);
                tickTimer = 0f;
            }
        }
        else
        {
            lineRenderer.enabled = false;
            tickTimer = 0f;
            canShoot = false;
        }
    }

    private Unit GetNearestFlyingTarget()
    {
        Unit closest = null;
        float shortestDist = Mathf.Infinity;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D col in hits)
        {
            Unit unit = col.GetComponent<Unit>();
            if (unit == null || unit.Type != UnitType.Flying) continue;

            float dist = Vector2.Distance(transform.position, unit.transform.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                closest = unit;
            }
        }

        return closest;
    }

    public void EnableShooting()
    {
        canShoot = true;
    }
}
