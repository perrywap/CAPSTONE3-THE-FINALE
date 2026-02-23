using UnityEngine;

public class GlassBeast : EnemyBase
{
    [Header("References")]
    [SerializeField] private GameObject shardBeastPrefab;
    [SerializeField] private int numToSpawn = 3;

    public override void Die()
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            GameObject shardBeast = Instantiate(shardBeastPrefab);
        }
        base.Die();
        
    }
}
