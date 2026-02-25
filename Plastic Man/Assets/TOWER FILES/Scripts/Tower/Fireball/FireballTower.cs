using UnityEngine;

public class FireballTower : Tower
{
    [Header("Fireball Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Animation")]
    [SerializeField] private FireballTowerAnimator animator;

    private void Start()
    {
        if (animator != null)
        {
            animator.OnGlowFrameReached += () =>
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
            if (fireCooldown <= 0f)
            {
                fireCooldown = 1f / fireRate;
            }

            animator.PlayAttack();
        }
        else
        {
            animator.PlayIdle();
        }
    }

    public override void Attack(Unit target)
    {
        if (fireballPrefab != null && firePoint != null && target != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();

            if (projectile != null)
            {
                projectile.SetTarget(target, damage);
            }
        }
    }
}