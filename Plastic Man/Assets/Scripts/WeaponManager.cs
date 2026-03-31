using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Transform[] slots;
    [SerializeField] private Player player;

    [Header("Attributes")]
    [SerializeField] private float popupValue = 20f;

    private Vector3[] originalPositions;
    private int activeIndex = -1;
    private int lastActiveIndex = -1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        originalPositions = new Vector3[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            originalPositions[i] = slots[i].position;
        }

        UpdateSlotVisibility();
    }

    private void Update()
    {
        if (NPCDialogue.IsTalking) return;

        WeaponChange();
        HandleWeaponSlot();
    }

    private void HandleWeaponSlot()
    {
        if (activeIndex == lastActiveIndex) return;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].position = originalPositions[i];
        }

        if (activeIndex != -1 &&
            activeIndex < slots.Length &&
            activeIndex < weapons.Length &&
            weapons[activeIndex] != null)
        {
            slots[activeIndex].position += new Vector3(0, popupValue, 0);
        }

        lastActiveIndex = activeIndex;
    }

    private void WeaponChange()
    {
        if (weapons == null || weapons.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) SetWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SetWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SetWeapon(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SetWeapon(3);
    }

    private void SetWeapon(int index)
    {
        if (index >= weapons.Length)
        {
            Debug.LogWarning($"Weapon index {index} is out of bounds.");
            PlayerCombat.Instance.ChangeWeapon(null);
            activeIndex = -1;
            return;
        }

        if (weapons[index] != null)
        {
            PlayerCombat.Instance.ChangeWeapon(weapons[index]);
            activeIndex = index;
        }
        else
        {
            PlayerCombat.Instance.ChangeWeapon(null);
            Debug.LogWarning($"No weapon assigned on weapons index {index}");
            activeIndex = -1;
        }

        UpdateSlotVisibility();
    }

    private void UpdateSlotVisibility()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            bool hasWeapon = (i < weapons.Length && weapons[i] != null);
            slots[i].gameObject.SetActive(hasWeapon);
        }
    }
}