using UnityEngine;

public class GearLauncher : WeaponBase
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private Transform muzzleFlashPos;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private float shootVolume = 0.5f;

    protected override void Fire(Vector3 origin, Vector3 direction)
    {
        if (shootSfx != null)
            SfxManager.instance.PlaySFX(shootSfx, shootVolume);

        if (muzzleFlashPrefab != null && muzzleFlashPos != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzleFlashPos.position, muzzleFlashPos.rotation);
            Destroy(flash, 0.05f);
        }

        GameObject gear = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        Projectile2D projectile = gear.GetComponent<Projectile2D>();
        if (projectile != null)
        {
            projectile.damage = damage;
            projectile.Launch(direction);
        }
    }
}