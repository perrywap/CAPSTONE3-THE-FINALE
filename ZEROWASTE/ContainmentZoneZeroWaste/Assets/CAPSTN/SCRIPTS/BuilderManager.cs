using UnityEngine;
using static System.Collections.Specialized.BitVector32;

public enum BuildStationType
{
    Printer,
    Forger,
    Modler
}

[System.Serializable]
public class TierCost
{
    public ResourceEntry[] costs;
}

[System.Serializable]
public struct BuildStation
{
    public BuildStationType stationType;
    public int tier;
    public TierCost[] tierCosts;   
}

public class BuilderManager : MonoBehaviour
{
    public static BuilderManager Instance { get; private set; }

    [Header("BUILD STATIONS")]
    [SerializeField] private BuildStation[] buildStations;

    [Header("UPGRADE BUTTONS")]
    [SerializeField] private GameObject[] upgradeButtons;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializeStations();
    }

    private void Update()
    {
        ShowUpgradeButtons();
    }

    void InitializeStations()
    {
        for (int i = 0; i < buildStations.Length; i++)
        {
            buildStations[i].tier = 1;
        }
    }

    private void ShowUpgradeButtons()
    {
        int count = Mathf.Min(buildStations.Length, upgradeButtons.Length);

        for (int i = 0; i < count; i++)
        {
            if (buildStations[i].tier >= 3)
            {
                upgradeButtons[i].SetActive(false);
                continue; 
            }

            int tierIndex = buildStations[i].tier - 1;

            // SAFETY CHECK
            if (tierIndex < 0 || tierIndex >= buildStations[i].tierCosts.Length)
            {
                upgradeButtons[i].SetActive(false);
                continue;
            }

            ResourceEntry[] cost =
                buildStations[i].tierCosts[tierIndex].costs;

            upgradeButtons[i].SetActive(
                GameManager.Instance.CanSpend(cost)
            );
        }
    }

    public bool IsStationTier3(BuildStationType type)
    {
        foreach (BuildStation station in buildStations)
        {
            if (station.stationType == type)
                return station.tier >= 3;
        }
        return false;
    }

    public void UpgradeStation(BuildStationType type)
    {
        for (int i = 0; i < buildStations.Length; i++)
        {
            if (buildStations[i].stationType != type)
                continue;

            BuildStation station = buildStations[i];

            if (station.tier >= 3)
            {
                Debug.Log(type + " already max tier.");
                return;
            }

            ResourceEntry[] cost =
                station.tierCosts[station.tier - 1].costs;

            if (!GameManager.Instance.CanSpend(cost))
            {
                Debug.Log("Not enough resources.");
                return;
            }

            GameManager.Instance.SpendMaterials(cost);

            station.tier++;
            buildStations[i] = station;

            PlayerController.Instance.RefreshSkillButtons();

            Debug.Log(type + " upgraded to Tier " + station.tier);
            return;
        }
    }

    public int GetStationTier(BuildStationType type)
    {
        foreach (BuildStation station in buildStations)
        {
            if (station.stationType == type)
                return station.tier;
        }
        return 1;
    }
}
