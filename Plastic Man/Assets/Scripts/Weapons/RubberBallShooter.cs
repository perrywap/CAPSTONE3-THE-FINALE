using UnityEngine;

public class RubberBallShooter : WeaponBase
{
    [Header("References")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;

    [Header("VFX")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private Transform muzzleFlashPos;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private float shootVolume = 0.5f;

    protected override void Fire(Vector3 origin, Vector3 direction)
    {
        if (shootSfx != null)
        {
            SfxManager.instance.PlaySFX(shootSfx, shootVolume);
        }

        if (muzzleFlashPrefab != null && muzzleFlashPos != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzleFlashPos.position, muzzleFlashPos.rotation);
            Destroy(flash, 0.05f);
        }

        GameObject rubberBall = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);

        RubberBallProjectile projectile = rubberBall.GetComponent<RubberBallProjectile>();
        if (projectile != null)
        {
            projectile.damage = damage;
            projectile.Launch(direction);
        }
    }
}