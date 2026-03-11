using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] _spawnAreas; 
    [SerializeField] private float _timeBetweenSpawns; 
    [SerializeField] private EnemyAI _enemyPrefab; 
    [SerializeField] private int _maxEnemiesAtOnce = 20; 
    [SerializeField] private int _maxEnemiesAlive = 20; 

    private float _lastSpawnTime;
    private GameObject _areaToUse;
    private Collider2D _areaCollider;
    private Bounds _bounds;
    private List<GameObject> _activeEnemies = new List<GameObject>();

    void Update()
    {
        if (_activeEnemies.Count < _maxEnemiesAlive && Time.time > _lastSpawnTime + _timeBetweenSpawns)
        {
            _lastSpawnTime = Time.time;
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        int enemiesToSpawn = Random.Range(1, _maxEnemiesAtOnce + 1);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GetRandomCollider();
            Vector2 spawnPosition = RandomPointInBox(_bounds);
            GameObject enemy = Instantiate(_enemyPrefab.gameObject, spawnPosition, Quaternion.identity);
            _activeEnemies.Add(enemy);
        }
    }

    private void GetRandomCollider()
    {
        _areaToUse = _spawnAreas[Random.Range(0, _spawnAreas.Length)];
        _areaCollider = _areaToUse.GetComponent<Collider2D>();
        _bounds = _areaCollider.bounds;
    }

    private Vector2 RandomPointInBox(Bounds bounds)
    {
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        if (_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Remove(enemy);
        }
    }
}
