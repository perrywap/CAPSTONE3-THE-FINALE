using UnityEngine;

public class PlayerModuleManager : MonoBehaviour
{
    public static PlayerModuleManager Instance { get; private set; }

    [Header("Module Holders")]
    [SerializeField] private Transform headModuleHolder;
    [SerializeField] private Transform bodyModuleHolder;

    private GameObject equippedHeadModule;
    private GameObject equippedBodyModule;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EquipSavedModules();
    }

    public void EquipSavedModules()
    {
        if (Loadout.Instance == null)
            return;

        EquipHeadModule(Loadout.Instance.HeadModule);
        EquipBodyModule(Loadout.Instance.BodyModule);
    }

    public void EquipHeadModule(GameObject modulePrefab)
    {
        if (equippedHeadModule != null)
            Destroy(equippedHeadModule);

        equippedHeadModule = null;

        if (modulePrefab == null || headModuleHolder == null)
            return;

        equippedHeadModule = Instantiate(modulePrefab, headModuleHolder);
        equippedHeadModule.transform.localPosition = Vector3.zero;
        equippedHeadModule.transform.localRotation = Quaternion.identity;
        equippedHeadModule.transform.localScale = Vector3.one;
    }

    public void EquipBodyModule(GameObject modulePrefab)
    {
        if (equippedBodyModule != null)
            Destroy(equippedBodyModule);

        equippedBodyModule = null;

        if (modulePrefab == null || bodyModuleHolder == null)
            return;

        equippedBodyModule = Instantiate(modulePrefab, bodyModuleHolder);
        equippedBodyModule.transform.localPosition = Vector3.zero;
        equippedBodyModule.transform.localRotation = Quaternion.identity;
        equippedBodyModule.transform.localScale = Vector3.one;
    }
}