

//using UnityEngine;

//public class WeaponSlot : MonoBehaviour
//{
//    [SerializeField] private int slotIndex;
//    [SerializeField] private bool equipOnDrop = true;

//    public void TrySetWeapon(PickUps pickup)
//    {
//        if (pickup == null)
//            return;

//        if (pickup.WeaponPrefab == null)
//        {
//            pickup.RestorePickup();
//            return;
//        }

//        if (WeaponManager.Instance == null)
//        {
//            pickup.RestorePickup();
//            return;
//        }

//        WeaponManager.Instance.SetWeaponToSlot(slotIndex, pickup.WeaponPrefab);

//        if (equipOnDrop)
//            WeaponManager.Instance.EquipWeaponSlot(slotIndex);

//        Destroy(pickup.gameObject);
//    }
//}

using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private bool equipOnDrop = true;

    public void TrySetWeapon(PickUps pickup)
    {
        if (pickup == null)
            return;

        if (pickup.WeaponPrefab == null)
        {
            pickup.RestorePickup();
            return;
        }

        if (WeaponManager.Instance == null)
        {
            pickup.RestorePickup();
            return;
        }

        if (pickup.WeaponPrefab.GetComponent<PlayerModule>() != null)
        {
            pickup.RestorePickup();
            return;
        }

        WeaponBase weapon = pickup.WeaponPrefab.GetComponent<WeaponBase>();

        if (weapon == null)
        {
            pickup.RestorePickup();
            return;
        }

        WeaponManager.Instance.SetWeaponToSlot(slotIndex, pickup.WeaponPrefab);

        if (equipOnDrop)
            WeaponManager.Instance.EquipWeaponSlot(slotIndex);

        Destroy(pickup.gameObject);
    }
}