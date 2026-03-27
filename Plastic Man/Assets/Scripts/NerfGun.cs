using UnityEngine;

public class NerfGun : WeaponBase
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private Transform muzzleFlashPos;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSfx; 
    [SerializeField] private float shootVolume = 0.5f;

    private void Update()
    {
        animator.SetBool("isFiring", false);
    }

    protected override void Fire(Vector3 origin, Vector3 direction)
    {
        animator.SetBool("isFiring", true);
        CameraShake.Instance.RifleShake();

        // Play shooting sound
        if (shootSfx != null)
        {
            SfxManager.instance.PlaySFX(shootSfx, shootVolume);
        }

        // Muzzle flash
        GameObject flash = Instantiate(muzzleFlashPrefab, muzzleFlashPos.position, muzzleFlashPos.rotation);
        Destroy(flash, 0.05f);

        // Spawn projectile
        GameObject bullet = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        Projectile2D projectile = bullet.GetComponent<Projectile2D>();
        if (projectile != null)
        {
            projectile.damage = damage;
            projectile.Launch(direction);
        }
    }
}