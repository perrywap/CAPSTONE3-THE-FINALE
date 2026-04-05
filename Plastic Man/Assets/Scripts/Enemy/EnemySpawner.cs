using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] _spawnAreas;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _timeBetweenSpawns = 2f;
    [SerializeField] private int _maxEnemiesAlive = 15;

    [Header("Proximity Settings")]
    [SerializeField] private float _minSpawnDistance = 10f;
    [SerializeField] private float _maxSpawnDistance = 25f;

    private Transform _playerTransform;
    private Camera _mainCamera;
    private float _lastSpawnTime;
    private List<GameObject> _activeEnemies = new List<GameObject>();

    void Start()
    {
        _mainCamera = Camera.main;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    void Update()
    {
        _activeEnemies.RemoveAll(item => item == null);

        if (_activeEnemies.Count < _maxEnemiesAlive && Time.time > _lastSpawnTime + _timeBetweenSpawns)
        {
            _lastSpawnTime = Time.time;
            SpawnSingleEnemy();
        }
    }

    private void SpawnSingleEnemy()
    {
        if (_playerTransform == null) return;

        Vector3 spawnPos = GetValidPointFromBoxes();

        if (spawnPos != Vector3.zero)
        {
            GameObject enemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
            _activeEnemies.Add(enemy);
        }
    }

    private Vector3 GetValidPointFromBoxes()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject randomArea = _spawnAreas[Random.Range(0, _spawnAreas.Length)];
            Collider2D col = randomArea.GetComponent<Collider2D>();

            if (col == null) continue;

            Bounds b = col.bounds;
            Vector3 randomPoint = new Vector3(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y),
                0f
            );

            float distToPlayer = Vector2.Distance(randomPoint, _playerTransform.position);

            if (distToPlayer >= _minSpawnDistance && distToPlayer <= _maxSpawnDistance)
            {
                Vector3 screenPoint = _mainCamera.WorldToViewportPoint(randomPoint);
                bool isOffScreen = screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;

                if (isOffScreen)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
                    {
                        return hit.position;
                    }
                }
            }
        }
        return Vector3.zero;
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        if (_activeEnemies.Contains(enemy)) _activeEnemies.Remove(enemy);
    }
}