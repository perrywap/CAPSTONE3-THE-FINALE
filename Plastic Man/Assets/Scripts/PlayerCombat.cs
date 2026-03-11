using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject equippedWeapon;
    [SerializeField] private Transform weaponAttach;
    [SerializeField] private Transform aimTransform;

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

    private void HandleAiming()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 localScale = Vector3.one;
        if (angle > 90 || angle < -90)
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
            if (equippedWeapon == null)
                return;

            Vector3 mousePosition = Input.mousePosition;
            equippedWeapon.GetComponent<WeaponBase>().TryFire(mousePosition);
        }
    }

    public void ChangeWeapon(GameObject weap)
    {
        if (equippedWeapon != null)
            Destroy(equippedWeapon.gameObject);

        equippedWeapon = Instantiate(weap, weaponAttach);
        equippedWeapon.transform.parent = weaponAttach.transform;
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
