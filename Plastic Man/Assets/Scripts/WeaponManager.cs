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
    }

    private void Update()
    {
        WeaponChange();
    }

    private void WeaponChange()
    {
        int newActiveIndex = activeIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerCombat.Instance.ChangeWeapon(weapons[0]);
            newActiveIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerCombat.Instance.ChangeWeapon(weapons[1]);
            newActiveIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayerCombat.Instance.ChangeWeapon(weapons[2]);
            newActiveIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayerCombat.Instance.ChangeWeapon(weapons[3]);
            newActiveIndex = 3;
        }
        if (newActiveIndex != activeIndex)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].position = originalPositions[i];
            }

            if (newActiveIndex != -1)
            {
                slots[newActiveIndex].position += new Vector3(0, popupValue, 0);
            }

            activeIndex = newActiveIndex;
        }
    }
}