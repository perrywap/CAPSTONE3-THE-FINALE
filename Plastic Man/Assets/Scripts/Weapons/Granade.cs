using UnityEngine;
using System.Collections;

public class Granade : WeaponBase
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float maxThrowDistance = 5f;
    [Header("Audio")]
    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private float shootVolume = 0.5f;

    [Header("Visual")]
    [SerializeField] private float hideDuration = 2f;

    private SpriteRenderer _spriteRenderer;
    private bool canFire = true;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Fire(Vector3 origin, Vector3 direction)
    {
        if (!canFire) return;

        canFire = false;

        if (shootSfx != null && SfxManager.instance != null)
        {
            SfxManager.instance.PlaySFX(shootSfx, shootVolume);
        }

        // FIXED 2D MOUSE POSITION (clean version)
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -Camera.main.transform.position.z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreen);
        worldPos.z = 0f;

        // Clamp distance
        Vector3 directionToMouse = worldPos - origin;
        float distance = directionToMouse.magnitude;

        if (distance > maxThrowDistance)
        {
            worldPos = origin + directionToMouse.normalized * maxThrowDistance;
        }

        GameObject bullet = Instantiate(projectilePrefab, origin, Quaternion.identity);

        GrenadeProjectile grenade = bullet.GetComponent<GrenadeProjectile>();
        if (grenade != null)
        {
            grenade.SetTarget(worldPos);
        }

        StartCoroutine(HideAndCooldown());
    }

    private IEnumerator HideAndCooldown()
    {
        if (_spriteRenderer != null)
            _spriteRenderer.enabled = false;

        yield return new WaitForSeconds(hideDuration);

        if (_spriteRenderer != null)
            _spriteRenderer.enabled = true;

        canFire = true;
    }
}