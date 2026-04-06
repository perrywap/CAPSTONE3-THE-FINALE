using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoguePrinterSpawner : MonoBehaviour
{
    public static RoguePrinterSpawner Instance { get; private set; }

    [Header("Reference")]
    [SerializeField] private GameObject[] spawnableEnemies;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private SpriteRenderer frontSprite;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int spawnCount;
    [SerializeField] private int maxSpawnCount;

    [Header("Generator & Sequence Settings")]
    public List<GameObject> generators;
    [SerializeField] private CutsceneTrigger _enemiesAliveCutscene;
    [SerializeField] private CutsceneTrigger _enemiesDeadCutscene;
    [SerializeField] private GameObject cleanPrinter;
    [SerializeField] private float animationDuration = 2f;

    public bool isDefeated;
    private float timer;
    private Animator animator;
    private bool _isExploding = false;

    private void Awake()
    {
        Instance = this;
    }

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

        if (generators.Count <= 0 && !_isExploding)
        {
            _isExploding = true;

            // --- THE FIX: Instantly tell the game it's defeated, just like your original code! ---
            isDefeated = true;

            if (GameManager.Instance != null && GameManager.Instance.ActiveEnemyCount > 0)
            {
                if (_enemiesAliveCutscene != null) _enemiesAliveCutscene.PlayFromGameManager();
            }
            else
            {
                if (GameManager.Instance != null)
                {
                    // --- THE SMART SWITCH ---

                    // PHASE 1: Is this Level 5? (Does the GameManager have a Statue Reveal cutscene?)
                    if (GameManager.Instance.MidLevelCutscene != null)
                    {
                        if (_enemiesDeadCutscene != null)
                        {
                            _enemiesDeadCutscene.SetNextCutscene(GameManager.Instance.MidLevelCutscene);
                        }
                        GameManager.Instance.NotifyMidLevelCutsceneHandled();

                        if (_enemiesDeadCutscene != null) _enemiesDeadCutscene.PlayFromGameManager();
                    }

                    // PHASE 2: Is this Level 1-4? (Normal behavior!)
                    else
                    {
                        if (_enemiesDeadCutscene != null)
                        {
                            // Hand the cutscene directly to the GameManager so the Win Panel pops up FIRST!
                            GameManager.Instance.TriggerWinSequence(_enemiesDeadCutscene);
                        }
                    }
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

        // Let the GameObject turn off properly without delaying the Win Panel!
        gameObject.SetActive(false);
    }
}