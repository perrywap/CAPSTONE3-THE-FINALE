using UnityEngine;

public class LootInventory : MonoBehaviour
{
    public static LootInventory Instance { get; private set; }

    [Header("Plastic Counts")]
    public int polyethyleneCount;
    public int acrylicCount;
    public int polycarbonateCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddPlastic(PlasticType type, int amount)
    {
        switch (type)
        {
            case PlasticType.Polyethylene:
                polyethyleneCount += amount;
                break;
            case PlasticType.Acrylic:
                acrylicCount += amount;
                break;
            case PlasticType.Polycarbonate:
                polycarbonateCount += amount;
                break;
        }

        Debug.Log($"Inventory added {amount} of {type}. PE: {polyethyleneCount}, AC: {acrylicCount}, PC: {polycarbonateCount}");
    }
}