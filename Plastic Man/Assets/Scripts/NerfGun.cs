using UnityEngine;

public class NerfGun : WeaponBase
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;

    protected override void Fire(Vector3 origin, Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject bullet = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        Projectile2D projectile = bullet.GetComponent<Projectile2D>();
        projectile.damage = damage;

        if (projectile != null)
        {
            projectile.Launch(direction);
        }
    }
}