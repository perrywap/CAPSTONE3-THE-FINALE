using UnityEngine;

public enum ResourceType
{
    PLASTIC,
    METAL,
    GLASS
}

// New Enum to distinguish between Towers and Traps
public enum PlacementType
{
    Node,
    Path
}

[System.Serializable]
public struct ResourceEntry
{
    public ResourceType resourceType;
    public int resourceCost;
}

[CreateAssetMenu(fileName = "TowerData", menuName = "ScriptableObjectData/TowerData")]
public class TowerData : ScriptableObject
{
    [Header("General Stats")]
    public string towerName;
    public PlacementType placementType = PlacementType.Node; // Default to Node

    [Header("Visuals & Prefabs")]
    public ResourceEntry[] resourceEntry; 
    public Sprite towerGhost; 
    public GameObject towerPrefab; 
}