using NUnit.Framework;
using UnityEngine;

[System.Serializable] public struct EnemySpawnInfo
{
    public GameObject _enemyPrefab;
    public int _spawnCount;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "ScriptableObjectData/WaveData", order = 0)]
public class WaveData : ScriptableObject
{
    public EnemySpawnInfo[] _enemiesToSpawn;

    public int GetTotalEnemies()
    {
        int totalEnemies = 0;

        for (int i = 0; i < _enemiesToSpawn.Length; i++)
        {
            totalEnemies = +_enemiesToSpawn[i]._spawnCount;
        }

        Debug.Log("Total enemies to spawn in current wave is " +  totalEnemies);
        return totalEnemies;
    }
}
