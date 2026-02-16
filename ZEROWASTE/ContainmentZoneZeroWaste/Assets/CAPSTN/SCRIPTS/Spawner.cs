using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    #region VARIABLES
    public static Spawner Instance { get; private set; }

    [Header("SPAWNER INFO")]
    // FOR MULTIPLE LANE PURPOSES, SPAWNER
    // CAN HAVE ITS OWN SET OF WAYPOINTS
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private BoxCollider2D boxCol;
    
    // WAVE SETTINGS
    [SerializeField] private WaveData[] waves;

    // CURRENT WAVE SETTINGS
    [Header("CURRENT WAVE INFO")]
    [SerializeField] private EnemySpawnInfo[] currentWaveEnemies;
    [SerializeField] private int currentSpawnCount;
    [SerializeField] private int currentWaveIndex;
    [SerializeField] private float spawnInterval;

    public WaveData[] Wave { get { return waves; } }
    public int CurrentWaveIndex { get { return currentWaveIndex + 1; } }
    #endregion

    #region UNITY METHODS
    private void Start()
    {
        Instance = this;

        currentWaveIndex = 0;
    }
    #endregion

    #region METHODS
    public void StartWave()
    {
        GameManager.Instance.isWaveRunning = true;
        GameManager.Instance.WaveText.text = $"Wave: {currentWaveIndex + 1}/{GameManager.Instance.totalWaveCount}";

        currentWaveEnemies = waves[currentWaveIndex]._enemiesToSpawn;
        StartCoroutine(StartSpawner());
    }

    private Vector2 GetRandomPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    private void SpawnEnemy(GameObject enemyToSpawn)
    {
        Vector2 spawnPos = GetRandomPointInBounds(boxCol.bounds);

        GameObject enemyGO = Instantiate(enemyToSpawn, this.transform.position, Quaternion.identity);

        // SET SPAWNER'S WAYPOINTS TO THE ENEMY WAYPOINT VARIBLE 
        WaypointMovement waypointMovement = enemyGO.GetComponent<WaypointMovement>();
        waypointMovement.Waypoints = waypoints;

        // THIS SHOULD BE ADDED TO GAME MANAGER > SPAWNED ENEMY LIST
        GameManager.Instance.enemies.Add(enemyGO);
    }

    private IEnumerator StartSpawner()
    {
        for(int i = 0; i < currentWaveEnemies.Length; i++)
        {
            currentSpawnCount = currentWaveEnemies[i]._spawnCount;
            for(int j = 0; j < currentSpawnCount; j++)
            {
                yield return new WaitForSeconds(spawnInterval);
                SpawnEnemy(currentWaveEnemies[i]._enemyPrefab);
            }
        }

        OnSpawnerFinished();
    }

    private void OnSpawnerFinished()
    {
        GameManager.Instance.isWaveRunning = false;

        currentWaveIndex++;
        if (currentWaveIndex == waves.Length)
            return;
    }
    #endregion
}
