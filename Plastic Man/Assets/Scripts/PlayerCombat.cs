using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject equippedWeapon;
    [SerializeField] private Transform aimTransform;

    private WeaponBase weapon;

    private void Awake()
    {
        aimTransform = transform.Find("Aim");

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
        Vector3 mousePosition = Input.mousePosition;

        Vector3 aimDirection = (mousePosition - transform.position).normalized;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        
        aimTransform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 localScale = Vector3.one;
        
        if (angle > 90 || angle < -90)
            localScale.y = -1f;
        else
            localScale.y = +1f;
        
        aimTransform.localScale = localScale;
    }

    private void HandleShooting()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if (equippedWeapon == null)
                return;

            Vector3 mousePosition = Input.mousePosition;

        }
    }
}
