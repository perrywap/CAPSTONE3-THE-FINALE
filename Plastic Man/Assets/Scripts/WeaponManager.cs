using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Transform[] slots;
    [SerializeField] private Image[] weaponIcons;
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

        RefreshWeaponUI();
    }

    private void Update()
    {
        if (NPCDialogue.IsTalking) return;

        WeaponChange();
        HandleWeaponSlot();
        HandleWeaponIcons();
    }

    private void HandleWeaponIcons()
    {
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            if (i < weapons.Length && weapons[i] != null)
            {
                SpriteRenderer sr = weapons[i].GetComponent<SpriteRenderer>();

                if (sr != null)
                {
                    weaponIcons[i].sprite = sr.sprite;
                    weaponIcons[i].color = Color.white;
                }
            }
            else
            {
                weaponIcons[i].sprite = null;
                weaponIcons[i].color = new Color(1f, 1f, 1f, 0f);
            }
        }
    }

    private void HandleWeaponSlot()
    {
        if (activeIndex == lastActiveIndex) return;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].position = originalPositions[i];
        }

        if (activeIndex != -1 &&
            activeIndex < slots.Length)
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
            activeIndex = -1;
            PlayerCombat.Instance.ChangeWeapon(null);
            RefreshWeaponUI();
            return;
        }

        activeIndex = index;

        if (weapons[index] != null)
        {
            PlayerCombat.Instance.ChangeWeapon(weapons[index]);
        }
        else
        {
            PlayerCombat.Instance.ChangeWeapon(null);
        }

        RefreshWeaponUI();
    }

    private void UpdateSlotVisibility()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(true);
        }
    }

    private void RefreshWeaponUI()
    {
        UpdateSlotVisibility();
        HandleWeaponIcons();
        HandleWeaponSlot();
    }

    public void SetWeaponToSlot(int index, GameObject weapon)
    {
        if (index < 0 || index >= weapons.Length)
            return;

        weapons[index] = weapon;
        UpdateSlotVisibility();
        HandleWeaponIcons();
        HandleWeaponSlot();
    }
}