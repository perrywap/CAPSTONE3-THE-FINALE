using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    [Header("Loot Prefabs")]
    [SerializeField] private GameObject _polyethyleneBits;   
    [SerializeField] private GameObject _acrylicSheets;      
    [SerializeField] private GameObject _polycarbonateWire;   

    public void DropLoot(Vector3 position)
    {
        int roll = Random.Range(1, 101); 

        if (roll <= 55)
        {
            SpawnLoot(_polyethyleneBits, position);
        }
        else if (roll <= 55 + 35) 
        {
            SpawnLoot(_acrylicSheets, position);
        }
        else 
        {
            SpawnLoot(_polycarbonateWire, position);
        }
    }

    private void SpawnLoot(GameObject prefab, Vector3 position)
    {
        if (prefab != null)
        {
            Vector3 spawnPos = new Vector3(position.x, position.y, 0f);
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}