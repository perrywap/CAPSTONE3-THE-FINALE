using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoguePrinterSpawner : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject[] spawnableEnemies;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private SpriteRenderer frontSprite;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int spawnCount;
    [SerializeField] private int maxSpawnCount;

    [Header("Generator")]
    [SerializeField] private List<GameObject> generators;

    public bool isDefeated;
    private float timer;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        timer = spawnInterval;
    }
    private void Update()
    {
        if (isDefeated)
            return;

        timer -= Time.deltaTime;

        if(spawnCount != maxSpawnCount)
        {
            if (timer <= 0f)
            {
                frontSprite.sortingOrder = 11;
                animator.SetTrigger("spawn");
                timer = spawnInterval;
            }
        }

        for (int i = generators.Count - 1; i >= 0; i--)
        {
            if (generators[i] == null)
            {
                generators.RemoveAt(i);
            }
        }

        if (generators.Count <= 0)
        {
            isDefeated = true;
            animator.SetTrigger("destroyed");
        }
    }
    public void Spawn()
    {
        spawnCount++;
        frontSprite.sortingOrder = 1;
        int index = Random.Range(0, spawnableEnemies.Length);
        GameObject enemyGO = Instantiate(spawnableEnemies[index], spawnArea.position, Quaternion.identity);
    }
}