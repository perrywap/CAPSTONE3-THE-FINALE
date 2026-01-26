using UnityEngine;
using TMPro;
using System.Collections.Generic;
using NUnit.Framework;

public class GameManager : MonoBehaviour
{
    #region VARIABLES
    public static GameManager Instance { get; private set; }

    [Header("PLAYER HUD")]
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private TextMeshProUGUI plasticTxt;
    [SerializeField] private TextMeshProUGUI metalTxt;
    [SerializeField] private TextMeshProUGUI glassTxt;
    [SerializeField] private GameObject startWaveBtn;

    [Header("PLAYER STATS")]
    [SerializeField] private int health;

    [Header("INVENTORY")]
    private Dictionary<ResourceType, int> inventory = new Dictionary<ResourceType, int>();
    [SerializeField] private int plasticFilament;
    [SerializeField] private int metalScrap;
    [SerializeField] private int glassShard;

    [Header("SPAWNED UNITS")]
    public List<GameObject> _enemies = new List<GameObject>();

    public bool _isWaveRunning;

    public TextMeshProUGUI WaveText { get { return waveTxt; } set { waveTxt = value; } }
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        Instance = this;
        InitializeInventory();
    }

    private void Update()
    {
        HudManager();
        
    }
    #endregion

    #region METHODS
    private void HudManager()
    {
        plasticFilament = inventory[ResourceType.PLASTIC];
        metalScrap = inventory[ResourceType.METAL];
        glassShard = inventory[ResourceType.GLASS];

        healthTxt.text = health.ToString();
        plasticTxt.text = plasticFilament.ToString();
        metalTxt.text = metalScrap.ToString();
        glassTxt.text = glassShard.ToString();

        startWaveBtn.SetActive(_isWaveRunning || _enemies.Count != 0 ? false : true);
    }

    private void InitializeInventory()
    {
        inventory[ResourceType.PLASTIC] = plasticFilament;
        inventory[ResourceType.METAL] = metalScrap;
        inventory[ResourceType.GLASS] = glassShard;
    }

    public void AddMaterial(ResourceEntry entry)
    {
        if (!inventory.ContainsKey(entry.resourceType))
        {
            inventory[entry.resourceType] = 0;
        }

        inventory[entry.resourceType] += entry.resourceCost;
    }

    public void SpendMaterials(ResourceEntry[] matCosts)
    {
        // Safety check: make sure we can afford everything first
        foreach (ResourceEntry entry in matCosts)
        {
            if (!inventory.ContainsKey(entry.resourceType))
                return;

            if (inventory[entry.resourceType] < entry.resourceCost)
                return;
        }

        // Spend materials 
        foreach (ResourceEntry entry in matCosts)
        {
            inventory[entry.resourceType] -= entry.resourceCost;
        }
    }

    public bool CanSpend(ResourceEntry[] matEntries)
    {
        foreach (ResourceEntry entry in matEntries)
        {
            if (!inventory.ContainsKey(entry.resourceType) ||
                inventory[entry.resourceType] < entry.resourceCost)
                return false;
        }
        return true;
    }
    #endregion
}

