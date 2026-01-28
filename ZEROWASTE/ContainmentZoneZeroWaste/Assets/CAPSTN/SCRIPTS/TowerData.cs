using UnityEngine;

public enum ResourceType
{
    PLASTIC,
    METAL,
    GLASS
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
    public ResourceEntry[] resourceEntry; 
    public Sprite towerGhost; 
    public GameObject towerPrefab;
}
