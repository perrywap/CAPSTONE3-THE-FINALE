using System.Collections;
using System.Collections.Generic;
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

    [Header("Generator & Sequence Settings")]
    [SerializeField] private List<GameObject> generators;
    [SerializeField] private CutsceneTrigger _enemiesAliveCutscene;
    [SerializeField] private CutsceneTrigger _enemiesDeadCutscene;
    [SerializeField] private GameObject cleanPrinter;
    [SerializeField] private float animationDuration = 2f;

    public bool isDefeated;
    private float timer;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        timer = spawnInterval;
        if (cleanPrinter != null) cleanPrinter.SetActive(false);
    }

    private void Update()
    {
        if (isDefeated) return;

        timer -= Time.deltaTime;

        if (spawnCount != maxSpawnCount)
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
            if (generators[i] == null) generators.RemoveAt(i);
        }

        if (generators.Count <= 0)
        {
            isDefeated = true;

            if (GameManager.Instance != null && GameManager.Instance.ActiveEnemyCount > 0)
            {
                // Scenario A: Enemies alive! Play cutscene directly.
                if (_enemiesAliveCutscene != null) _enemiesAliveCutscene.PlayFromGameManager();
            }
            else
            {
                // Scenario B: Boss dies last! Tell GameManager to show Win Panel FIRST, then play cutscene!
                if (GameManager.Instance != null && _enemiesDeadCutscene != null)
                {
                    GameManager.Instance.TriggerWinSequence(_enemiesDeadCutscene);
                }
            }
        }
    }

    public void Spawn()
    {
        spawnCount++;
        frontSprite.sortingOrder = 1;
        int index = Random.Range(0, spawnableEnemies.Length);
        Instantiate(spawnableEnemies[index], spawnArea.position, Quaternion.identity);
    }

    public void TriggerExplosionAndSwap()
    {
        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        if (animator != null) animator.SetTrigger("destroyed");
        yield return new WaitForSecondsRealtime(animationDuration);
        if (cleanPrinter != null) cleanPrinter.SetActive(true);
        gameObject.SetActive(false);
    }
}