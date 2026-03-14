using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject equippedWeapon;
    [SerializeField] private Transform weaponAttach;
    [SerializeField] private Transform aimTransform;

    [SerializeField] private SpriteRenderer equippedWeaponSprite;

    private WeaponBase weapon;

    private void Awake()
    { 
        Instance = this;

        if (equippedWeapon != null)
            weapon = equippedWeapon.GetComponent<WeaponBase>();
    }

    private void Update()
    {
        HandleAiming();
        HandleShooting();
    }

    private void HandleAim2ing()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        bool aimingLeft = angle > 90f || angle < -90f;

        if (!aimingLeft)
        {
            angle = Mathf.Clamp(angle, -60f, 60f);
        }
        else
        {
            if (angle > 0)
                angle = Mathf.Clamp(angle, 120f, 180f);
            else
                angle = Mathf.Clamp(angle, -180f, -120f);
        }

        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 localScale = Vector3.one;
        localScale.y = aimingLeft ? -1f : 1f;
        aimTransform.localScale = localScale;
    }

    private void HandleAiming()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        float angle360 = angle < 0 ? angle + 360f : angle;

        bool aimingLeft = angle > 90f || angle < -90f;

        if (!aimingLeft)
        {
            angle = Mathf.Clamp(angle, -60f, 60f);
        }
        else
        {
            if (angle > 0)
                angle = Mathf.Clamp(angle, 120f, 180f);
            else
                angle = Mathf.Clamp(angle, -180f, -120f);
        }

        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 localScale = Vector3.one;
        localScale.y = aimingLeft ? -1f : 1f;
        aimTransform.localScale = localScale;

        if (angle360 > 30f && angle360 < 165f)
            equippedWeaponSprite.sortingOrder = 2;
        else
            equippedWeaponSprite.sortingOrder = 4;
    }

    private void HandleShooting()
    {
        if (equippedWeapon == null)
            return;

        if (Input.GetMouseButton(1))
        {
            //PlayerAnimation.Instance.isShooting = true;

            Vector3 mousePosition = GetMouseWorldPosition();
            weapon.TryFire(mousePosition);
        }
        else
        {
            //PlayerAnimation.Instance.isShooting = false;
        }
    }

    public void ChangeWeapon(GameObject weap)
    {
        if (equippedWeapon != null)
            Destroy(equippedWeapon.gameObject);

        equippedWeapon = Instantiate(weap, weaponAttach);
        equippedWeapon.transform.parent = weaponAttach;

        weapon = equippedWeapon.GetComponent<WeaponBase>();
    }

    #region MouseWorldPosition
    private static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    private static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }
    private static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    private static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
    #endregion
}
