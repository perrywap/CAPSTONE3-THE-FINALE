using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject equippedWeapon;
    [SerializeField] private Transform aimTransform;

    private void Awake()
    { 
        Instance = this;
    }

    private void Update()
    {
        if (NPCDialogue.IsTalking) return;
        if(PlayerHealth.Instance.isDead ) return;

        if (equippedWeapon == null)
            return;

        HandleAiming();
        HandleShooting();
    }

    private void HandleAiming()
    {

        float angle = PlayerLookAt.Instance.angle;
        bool aimingLeft = PlayerLookAt.Instance.isLookingLeft;

        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 localScale = Vector3.one;
        localScale.y = aimingLeft ? -1f : 1f;
        aimTransform.localScale = localScale;

        if (angle > 45f && angle < 165f)
            equippedWeapon.GetComponent<SpriteRenderer>().sortingOrder = 1;
        else
            equippedWeapon.GetComponent<SpriteRenderer>().sortingOrder = 5;
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            equippedWeapon.GetComponent<WeaponBase>().TryFire(mousePosition);
        }
    }

    public void ChangeWeapon(GameObject weap)
    {
        if (equippedWeapon != null)
            Destroy(equippedWeapon.gameObject);

        equippedWeapon = Instantiate(weap, aimTransform);
        equippedWeapon.transform.parent = aimTransform;
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
