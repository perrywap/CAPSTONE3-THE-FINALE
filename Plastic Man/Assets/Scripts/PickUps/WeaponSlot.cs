//using UnityEngine;
//using UnityEngine.EventSystems;

//public class WeaponSlot : MonoBehaviour
//{
//    [SerializeField] private int slotIndex;

//    public void OnDrop(PointerEventData eventData)
//    {
//        if (eventData.pointerDrag == null)
//            return;

//        PickUps pickup = eventData.pointerDrag.GetComponent<PickUps>();

//        if (pickup == null)
//            return;

//        if (pickup.WeaponPrefab == null)
//            return;

//        WeaponManager.Instance.SetWeaponToSlot(slotIndex, pickup.WeaponPrefab);
//        Destroy(pickup.gameObject);
//    } 
//} 

//using UnityEngine;

//public class WeaponSlot : MonoBehaviour
//{
//    [SerializeField] private int slotIndex;

//    public void TrySetWeapon(PickUps pickup)
//    {
//        if (pickup == null)
//            return;

//        if (pickup.WeaponPrefab == null)
//            return;

//        WeaponManager.Instance.SetWeaponToSlot(slotIndex, pickup.WeaponPrefab);
//        Destroy(pickup.gameObject);
//    }
//}

using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private int slotIndex;

    public void TrySetWeapon(PickUps pickup)
    {
        if (pickup == null)
            return;

        if (pickup.WeaponPrefab == null)
        {
            pickup.RestorePickup();
            return;
        }

        WeaponManager.Instance.SetWeaponToSlot(slotIndex, pickup.WeaponPrefab);
        Destroy(pickup.gameObject);
    }
}