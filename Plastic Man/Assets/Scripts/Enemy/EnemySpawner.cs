using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] _spawnAreas;
    [SerializeField] private float _timeBetweenSpawns = 2f;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _maxEnemiesAtOnce = 5;
    [SerializeField] private int _maxEnemiesAlive = 20;

    [Header("NavMesh Settings")]
    [SerializeField] private float _navMeshCheckRadius = 2f;

    private Camera _mainCamera;
    private float _lastSpawnTime;
    private List<GameObject> _activeEnemies = new List<GameObject>();

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        _activeEnemies.RemoveAll(item => item == null);

        if (_activeEnemies.Count < _maxEnemiesAlive && Time.time > _lastSpawnTime + _timeBetweenSpawns)
        {
            _lastSpawnTime = Time.time;
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        int spaceLeft = _maxEnemiesAlive - _activeEnemies.Count;
        int enemiesToSpawn = Mathf.Min(Random.Range(1, _maxEnemiesAtOnce + 1), spaceLeft);

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 spawnPosition = GetValidOffScreenPoint(planes);

            if (spawnPosition != Vector3.right * 9999f)
            {
                GameObject enemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
                _activeEnemies.Add(enemy);
            }
        }
    }

    private Vector3 GetValidOffScreenPoint(Plane[] planes)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            GameObject areaToUse = _spawnAreas[Random.Range(0, _spawnAreas.Length)];
            Collider2D areaCollider = areaToUse.GetComponent<Collider2D>();
            Bounds bounds = areaCollider.bounds;

            Vector3 randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                0f
            );

            if (!GeometryUtility.TestPlanesAABB(planes, new Bounds(randomPoint, Vector3.one)))
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, _navMeshCheckRadius, NavMesh.AllAreas))
                {
                    Vector3 finalPos = hit.position;
                    finalPos.z = 0f;
                    return finalPos;
                }
            }
        }

        return Vector3.right * 9999f;
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        if (_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Remove(enemy);
        }
    }
}