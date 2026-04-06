//using UnityEngine;

//public class PlayerCombat : MonoBehaviour
//{
//    public static PlayerCombat Instance { get; private set; }

//    [Header("References")]
//    [SerializeField] private GameObject equippedWeapon;
//    [SerializeField] private Transform aimTransform;

//    public GameObject EquippedWeapon { get { return equippedWeapon; } }

//    private void Awake()
//    { 
//        Instance = this;
//    }

//    private void Update()
//    {
//        if (NPCDialogue.IsTalking) return;
//        if(PlayerHealth.Instance.isDead ) return;

//        if (equippedWeapon == null)
//            return;

//        if (equippedWeapon.GetComponent<WeaponBase>() != null)
//        {
//            HandleAiming();
//            HandleShooting();
//        }

//    }

//    private void HandleAiming()
//    {

//        float angle = PlayerLookAt.Instance.angle;
//        bool aimingLeft = PlayerLookAt.Instance.isLookingLeft;

//        aimTransform.eulerAngles = new Vector3(0, 0, angle);

//        Vector3 localScale = Vector3.one;
//        localScale.y = aimingLeft ? -1f : 1f;
//        aimTransform.localScale = localScale;

//        if (angle > 45f && angle < 165f)
//            equippedWeapon.GetComponent<SpriteRenderer>().sortingOrder = 1;
//        else
//            equippedWeapon.GetComponent<SpriteRenderer>().sortingOrder = 5;
//    }

//    private void HandleShooting()
//    {
//        Vector3 mousePosition = GetMouseWorldPosition();
//        equippedWeapon.GetComponent<WeaponBase>().HandleInput(mousePosition);
//    }

//    public void ChangeWeapon(GameObject weap)
//    {
//        if (equippedWeapon != null)
//            Destroy(equippedWeapon.gameObject);

//        if(weap != null)
//        {
//            equippedWeapon = Instantiate(weap, aimTransform);
//            equippedWeapon.transform.parent = aimTransform;
//        }

//    }

//    #region MouseWorldPosition
//    private static Vector3 GetMouseWorldPosition()
//    {
//        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
//        vec.z = 0f;
//        return vec;
//    }

//    private static Vector3 GetMouseWorldPositionWithZ()
//    {
//        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
//    }
//    private static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
//    {
//        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
//    }
//    private static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
//    {
//        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
//        return worldPosition;
//    }
//    #endregion
//}

using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject equippedWeapon;
    [SerializeField] private Transform aimTransform;

    public GameObject EquippedWeapon => equippedWeapon;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (NPCDialogue.IsTalking) return;
        if (PlayerHealth.Instance.isDead) return;
        if (equippedWeapon == null) return;

        WeaponBase weaponBase = equippedWeapon.GetComponent<WeaponBase>();

        if (weaponBase != null)
        {
            HandleAiming();
            HandleShooting();
        }
    }

    private void HandleAiming()
    {
        float angle = PlayerLookAt.Instance.angle;
        bool aimingLeft = PlayerLookAt.Instance.isLookingLeft;

        aimTransform.eulerAngles = new Vector3(0f, 0f, angle);

        Vector3 localScale = Vector3.one;
        localScale.y = aimingLeft ? -1f : 1f;
        aimTransform.localScale = localScale;

        SpriteRenderer sr = equippedWeapon.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            if (angle > 45f && angle < 165f)
                sr.sortingOrder = 1;
            else
                sr.sortingOrder = 5;
        }
    }

    private void HandleShooting()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        WeaponBase weaponBase = equippedWeapon.GetComponent<WeaponBase>();

        if (weaponBase != null)
            weaponBase.HandleInput(mousePosition);
    }

    public void ChangeWeapon(GameObject weaponPrefab)
    {
        if (equippedWeapon != null)
            Destroy(equippedWeapon);

        equippedWeapon = null;

        if (weaponPrefab == null)
            return;

        equippedWeapon = Instantiate(weaponPrefab, aimTransform);
        equippedWeapon.transform.localPosition = Vector3.zero;
        equippedWeapon.transform.localRotation = Quaternion.identity;
        equippedWeapon.transform.localScale = Vector3.one;
    }

    private static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    private static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}
