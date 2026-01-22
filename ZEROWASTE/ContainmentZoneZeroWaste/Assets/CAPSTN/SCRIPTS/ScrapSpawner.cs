using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ScrapSpawner : MonoBehaviour
{
    public static ScrapSpawner Instance { get; private set; }
    [Header("References")]
    [SerializeField] private Transform washStation;
    [SerializeField] private Transform salvageStation;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private List<GameObject> scrapPrefabs = new List<GameObject>();

    private float randomSpawnTimer;
    private int randomIndex;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (scrapPrefabs.Count == 0)
        {
            Debug.LogWarning("Scrap prefab list is empty");
            return;
        }

        randomSpawnTimer = Random.Range(1f, 3f);
        randomIndex = Random.Range(0, scrapPrefabs.Count -1);
    }

    public void StartSpawner()
    {
        StartCoroutine(SpawnScrap());
    }

    private IEnumerator SpawnScrap()
    {
        yield return new WaitForSeconds(randomSpawnTimer);

        GameObject scrapGO = Instantiate(scrapPrefabs[randomIndex], transform.position, Quaternion.identity);
        Scraps scrap = scrapGO.GetComponent<Scraps>();
        scrap.InitializeScrap(washStation, salvageStation, waypoints);

        randomSpawnTimer = Random.Range(1f, 3f);
        randomIndex = Random.Range(0, scrapPrefabs.Count - 1);
    }
}
