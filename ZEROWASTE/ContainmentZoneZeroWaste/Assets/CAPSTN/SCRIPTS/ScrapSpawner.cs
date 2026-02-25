using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ScrapSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] srapPrefabs;
    [SerializeField] private BoxCollider2D dumpArea;

    [Header("Attributes")]
    [SerializeField] private float minSpawnRate = 4f, maxSpawnRate =  8f;

    private int index = 0;
    private float spawnTime = 0.1f;

    public void StartSpawner()
    {
        if (GameManager.Instance.isGameOver)
            return;

        StartCoroutine(SpawnScrapResource());
    }

    private Vector2 GetRandomPointInBounds(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    private void Randomize()
    {
        index = Random.Range(0, srapPrefabs.Length);
        spawnTime = Random.Range(minSpawnRate, maxSpawnRate);
    }

    private IEnumerator SpawnScrapResource()
    {
        while(true)
        {
            yield return new WaitForSeconds(spawnTime);
            Vector2 spawnPos = GetRandomPointInBounds(dumpArea.bounds);
            GameObject scrapGO = Instantiate(srapPrefabs[index], spawnPos, Quaternion.identity);

            if (!GameManager.Instance.isWaveRunning && GameManager.Instance.enemies.Count == 0)
                break;

            Randomize();
        }
    }
}
