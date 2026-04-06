//using UnityEngine;
//using UnityEngine.UI;

//public class ModuleSlot : MonoBehaviour
//{
//    [SerializeField] private PlayerModuleType slotType;
//    [SerializeField] private Image iconImage;
//    [SerializeField] private Image icon;


//    private void Start()
//    {
//        RefreshIcon();
//    }

//    public void TrySetModule(PickUps pickup)
//    {
//        if (pickup == null)
//            return;

//        if (pickup.WeaponPrefab == null)
//        {
//            pickup.RestorePickup();
//            return;
//        }

//        if (pickup.WeaponPrefab.GetComponent<WeaponBase>() != null)
//        {
//            pickup.RestorePickup();
//            return;
//        }

//        PlayerModule newModule = pickup.WeaponPrefab.GetComponent<PlayerModule>();

//        if (newModule == null)
//        {
//            pickup.RestorePickup();
//            return;
//        }

//        if (newModule.ModuleType != slotType)
//        {
//            pickup.RestorePickup();
//            return;
//        }

//        if (Loadout.Instance == null)
//        {
//            pickup.RestorePickup();
//            return;
//        }

//        RemoveCurrentModule();

//        switch (slotType)
//        {
//            case PlayerModuleType.HEAD:
//                Loadout.Instance.SetHeadModule(pickup.WeaponPrefab);

//                if (PlayerModuleManager.Instance != null)
//                    PlayerModuleManager.Instance.EquipHeadModule(pickup.WeaponPrefab);
//                break;

//            case PlayerModuleType.BODY:
//                Loadout.Instance.SetBodyModule(pickup.WeaponPrefab);

//                if (PlayerModuleManager.Instance != null)
//                    PlayerModuleManager.Instance.EquipBodyModule(pickup.WeaponPrefab);
//                break;

//            default:
//                pickup.RestorePickup();
//                return;
//        }

//        newModule.OnEquip();
//        SetIcon(newModule.IconSprite);
//        Destroy(pickup.gameObject);
//    }

//    private void RemoveCurrentModule()
//    {
//        if (Loadout.Instance == null)
//            return;

//        GameObject currentModuleObject = null;

//        switch (slotType)
//        {
//            case PlayerModuleType.HEAD:
//                currentModuleObject = Loadout.Instance.HeadModule;
//                break;

//            case PlayerModuleType.BODY:
//                currentModuleObject = Loadout.Instance.BodyModule;
//                break;
//        }

//        if (currentModuleObject == null)
//            return;

//        PlayerModule currentModule = currentModuleObject.GetComponent<PlayerModule>();

//        if (currentModule != null)
//            currentModule.OnUnequip();
//    }

//    public void RefreshIcon()
//    {
//        if (Loadout.Instance == null)
//        {
//            ClearIcon();
//            return;
//        }

//        GameObject moduleObject = null;

//        switch (slotType)
//        {
//            case PlayerModuleType.HEAD:
//                moduleObject = Loadout.Instance.HeadModule;
//                break;

//            case PlayerModuleType.BODY:
//                moduleObject = Loadout.Instance.BodyModule;
//                break;
//        }

//        if (moduleObject == null)
//        {
//            ClearIcon();
//            return;
//        }

//        PlayerModule module = moduleObject.GetComponent<PlayerModule>();

//        if (module == null)
//        {
//            ClearIcon();
//            return;
//        }

//        SetIcon(module.IconSprite);
//    }

//    private void SetIcon(Sprite icon)
//    {
//        if (iconImage == null)
//            return;

//        if (icon == null)
//        {
//            ClearIcon();
//            return;
//        }

//        iconImage.sprite = icon;
//        iconImage.color = Color.white;
//    }

//    private void ClearIcon()
//    {
//        if (iconImage == null)
//            return;

//        iconImage.sprite = null;
//        iconImage.color = new Color(1f, 1f, 1f, 0f);
//    }
//}

using UnityEngine;
using UnityEngine.UI;

public class ModuleSlot : MonoBehaviour
{
    [SerializeField] private PlayerModuleType slotType;
    [SerializeField] private Image iconImage;

    private void Start()
    {
        RefreshIcon();
    }

    public void TrySetModule(PickUps pickup)
    {
        if (pickup == null)
            return;

        if (pickup.WeaponPrefab == null)
        {
            pickup.RestorePickup();
            return;
        }

        if (pickup.WeaponPrefab.GetComponent<WeaponBase>() != null)
        {
            pickup.RestorePickup();
            return;
        }

        PlayerModule newModule = pickup.WeaponPrefab.GetComponent<PlayerModule>();

        if (newModule == null)
        {
            pickup.RestorePickup();
            return;
        }

        if (newModule.ModuleType != slotType)
        {
            pickup.RestorePickup();
            return;
        }

        if (Loadout.Instance == null)
        {
            pickup.RestorePickup();
            return;
        }

        RemoveCurrentModule();

        switch (slotType)
        {
            case PlayerModuleType.HEAD:
                Loadout.Instance.SetHeadModule(pickup.WeaponPrefab);

                if (PlayerModuleManager.Instance != null)
                    PlayerModuleManager.Instance.EquipHeadModule(pickup.WeaponPrefab);
                break;

            case PlayerModuleType.BODY:
                Loadout.Instance.SetBodyModule(pickup.WeaponPrefab);

                if (PlayerModuleManager.Instance != null)
                    PlayerModuleManager.Instance.EquipBodyModule(pickup.WeaponPrefab);
                break;

            default:
                pickup.RestorePickup();
                return;
        }

        newModule.OnEquip();
        SetIconFromPrefab(pickup.WeaponPrefab);

        Destroy(pickup.gameObject);
    }

    private void RemoveCurrentModule()
    {
        if (Loadout.Instance == null)
            return;

        GameObject currentModuleObject = null;

        switch (slotType)
        {
            case PlayerModuleType.HEAD:
                currentModuleObject = Loadout.Instance.HeadModule;
                break;

            case PlayerModuleType.BODY:
                currentModuleObject = Loadout.Instance.BodyModule;
                break;
        }

        if (currentModuleObject == null)
            return;

        PlayerModule currentModule = currentModuleObject.GetComponent<PlayerModule>();

        if (currentModule != null)
            currentModule.OnUnequip();
    }

    public void RefreshIcon()
    {
        if (Loadout.Instance == null)
        {
            ClearIcon();
            return;
        }

        GameObject moduleObject = null;

        switch (slotType)
        {
            case PlayerModuleType.HEAD:
                moduleObject = Loadout.Instance.HeadModule;
                break;

            case PlayerModuleType.BODY:
                moduleObject = Loadout.Instance.BodyModule;
                break;
        }

        if (moduleObject == null)
        {
            ClearIcon();
            return;
        }

        SetIconFromPrefab(moduleObject);
    }

    private void SetIconFromPrefab(GameObject prefab)
    {
        if (iconImage == null || prefab == null)
            return;

        SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();

        if (sr == null || sr.sprite == null)
        {
            ClearIcon();
            return;
        }

        iconImage.sprite = sr.sprite;
        iconImage.color = Color.white;
    }

    private void ClearIcon()
    {
        if (iconImage == null)
            return;

        iconImage.sprite = null;
        iconImage.color = new Color(1f, 1f, 1f, 0f);
    }
}