using System;
using UnityEngine;

public class PlayerAimWeapon : MonoBehaviour
{
    [SerializeField] private GameObject currentWeapon;
    private WeaponBase weaponBase;

    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Vector3 gunEndPointPosition;
        public Vector3 shootPosition;
    }

    private Transform aimTransform;
    private Transform aimGunEndPointTransform;

    private void Awake()
    {
        aimTransform = transform.Find("Aim") ;
        aimGunEndPointTransform = aimTransform.Find("GunEndPointPosition");

        if (currentWeapon != null)
            weaponBase = currentWeapon.GetComponent<WeaponBase>();
    }

    private void Update()
    {
        HandleAiming();
        HandleShooting();
    }

    private void HandleAiming()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 localScale = Vector3.one;
        if(angle > 90 || angle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = +1f;
        }
        aimTransform.localScale = localScale;
    }
 
    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (weaponBase == null) return;

            Vector3 mousePosition = GetMouseWorldPosition();

            weaponBase.TryFire(
                mousePosition
            );
        }
    }
    public void SetCurrentWeapon(GameObject weapon)
    {
        currentWeapon = weapon;
        if (currentWeapon != null)
            weaponBase = currentWeapon.GetComponent<WeaponBase>();
        else
            weaponBase = null;
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
