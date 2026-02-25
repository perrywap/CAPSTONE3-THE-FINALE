using System.Collections;
using UnityEngine;

public class DumperSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject dumperPrefab;
    [SerializeField] private BoxCollider2D[] spawnAreas;

    [Header("Attributes")]
    [SerializeField] private float minSpawnRate = 4f;
    [SerializeField] private float maxSpawnRate = 8f;

    private float spawnTime = 0.1f;

    public void StartSpawner()
    {
        if (GameManager.Instance.isGameOver)
            return;

        StartCoroutine(SpawnDumper());
    }

    private Vector2 GetRandomPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    private BoxCollider2D ChooseRandomSpawnArea()
    {
        if (spawnAreas == null || spawnAreas.Length == 0)
            return null;

        int randomIndex = Random.Range(0, spawnAreas.Length);
        return spawnAreas[randomIndex];
    }

    private IEnumerator SpawnDumper()
    {
        while (true)
        {
            spawnTime = Random.Range(minSpawnRate, maxSpawnRate);
            yield return new WaitForSeconds(spawnTime);

            BoxCollider2D chosenArea = ChooseRandomSpawnArea();
            if (chosenArea == null)
                yield break;

            Vector2 spawnPos = GetRandomPointInBounds(chosenArea.bounds);
            Instantiate(dumperPrefab, spawnPos, Quaternion.identity);

            if (!GameManager.Instance.isWaveRunning && GameManager.Instance.enemies.Count == 0)
                break;
        }
    }
}
