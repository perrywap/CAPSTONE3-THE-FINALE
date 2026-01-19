using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "ScriptableObjectData/TowerData")]
public class TowerData : ScriptableObject
{
    public int towerCost;
    public Sprite towerGhost; // For Towerghost rendering
    public GameObject towerPrefab; // For Instantiating

    
}
