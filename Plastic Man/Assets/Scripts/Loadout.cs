using UnityEngine;

public class Loadout : MonoBehaviour
{
    public static Loadout Instance { get; private set; }

    [Header("Player Modules")]
    [SerializeField] private GameObject headModule;
    [SerializeField] private GameObject bodyModule;

    [Header("Weapons")]
    [SerializeField] private GameObject[] weapons = new GameObject[4];
    [SerializeField] private float[] weaponEnergy = new float[4];
    [SerializeField] private int activeWeaponIndex = -1;

    public GameObject HeadModule => headModule;
    public GameObject BodyModule => bodyModule;
    public int ActiveWeaponIndex => activeWeaponIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (weapons == null || weapons.Length == 0)
            weapons = new GameObject[4];

        if (weaponEnergy == null || weaponEnergy.Length == 0)
            weaponEnergy = new float[4];
    }

    public void SetHeadModule(GameObject module)
    {
        headModule = module;
    }

    public void SetBodyModule(GameObject module)
    {
        bodyModule = module;
    }

    public GameObject GetWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
            return null;

        return weapons[index];
    }

    public void SetWeapon(int index, GameObject weaponPrefab)
    {
        if (index < 0 || index >= weapons.Length)
            return;

        weapons[index] = weaponPrefab;
    }

    public float GetWeaponEnergy(int index)
    {
        if (index < 0 || index >= weaponEnergy.Length)
            return 0f;

        return weaponEnergy[index];
    }

    public void SetWeaponEnergy(int index, float energy)
    {
        if (index < 0 || index >= weaponEnergy.Length)
            return;

        weaponEnergy[index] = energy;
    }

    public void SetActiveWeaponIndex(int index)
    {
        activeWeaponIndex = index;
    }

    public void ClearWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
            return;

        weapons[index] = null;
        weaponEnergy[index] = 0f;

        if (activeWeaponIndex == index)
            activeWeaponIndex = -1;
    }
}