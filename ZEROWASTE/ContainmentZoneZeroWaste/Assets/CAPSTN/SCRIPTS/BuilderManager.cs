using UnityEngine;

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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializeStations();
    }

    void InitializeStations()
    {
        for (int i = 0; i < buildStations.Length; i++)
        {
            buildStations[i].tier = 1;
        }
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

    public void UpgradePrinter()
    {
        Debug.Log("should upgrade");
        UpgradeStation(BuildStationType.Printer);
    }

    public void UpgradeForger()
    {
        UpgradeStation(BuildStationType.Forger);
    }

    public void UpgradeModler()
    {
        UpgradeStation(BuildStationType.Modler);
    }
}
