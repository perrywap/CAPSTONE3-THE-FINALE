using UnityEngine;

public class NerfGun : WeaponBase
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private Transform muzzleFlashPos;

    protected override void Fire(Vector3 origin, Vector3 direction)
    {
        CameraShake.Instance.RifleShake();
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject flash = Instantiate(muzzleFlashPrefab, muzzleFlashPos.position, muzzleFlashPos.rotation);

        Destroy(flash, 0.05f);

        GameObject bullet = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        Projectile2D projectile = bullet.GetComponent<Projectile2D>();
        projectile.damage = damage;

        if (projectile != null)
        {
            projectile.Launch(direction);
        }
    }
}