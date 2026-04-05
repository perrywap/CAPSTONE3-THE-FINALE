using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _winPanel;

    [Header("Level Boss (Optional)")]
    [Tooltip("Drag the Corrupted Printer Enemy here so the game waits for it to explode!")]
    [SerializeField] private RoguePrinterSpawner _levelBoss;

    [Header("Enemy Tracker")]
    [SerializeField] private List<GameObject> _enemies = new List<GameObject>();

    [Header("End Level Cutscene")]
    [Tooltip("Drag Cutscene_LateClear here!")]
    [SerializeField] private CutsceneTrigger _endLevelCutscene;

    private bool _isPaused = false;
    public bool IsPaused { get { return _isPaused; } }
    private bool _isGameOver = false;

    private bool _isGameCleared = false;
    public bool IsGameCleared { get { return _isGameCleared; } }

    // --- SAFETY LOCKS & STATE DATA ---
    private bool _hasStartedTracking = false;

    // Properties that the Printer needs to read
    public int ActiveEnemyCount => _enemies.Count;
    public bool IsWinPanelActive => _winPanel != null && _winPanel.activeSelf;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Prevent an instant-win if the list starts empty, but unlocks if enemies are in the Inspector
        if (_enemies.Count > 0)
        {
            _hasStartedTracking = true;
        }
    }

    void Update()
    {
        if (_isGameCleared) return;

        // --- SMART ESCAPE KEY ---
        if (Input.GetKeyDown(KeyCode.Escape) && !_isGameOver)
        {
            // 1. Close printer if it's open
            if (PrinterInteractable.IsInteracting)
            {
                PrinterInteractable printer = Object.FindFirstObjectByType<PrinterInteractable>();
                if (printer != null) printer.ClosePrinter();
            }
            // 2. Otherwise, pause the game normally (as long as we aren't in dialogue)
            else if (!NPCDialogue.IsTalking)
            {
                TogglePause();
            }
        }

        // Clean up any enemies that were destroyed
        _enemies.RemoveAll(item => item == null);

        // --- WIN LOGIC ---
        bool bossDefeated = _levelBoss == null || _levelBoss.isDefeated;

        // If enemies die last, the GameManager handles the win.
        // If the boss dies last, the Boss handles it and calls TriggerWinSequence directly!
        if (_hasStartedTracking && _enemies.Count <= 0 && bossDefeated)
        {
            TriggerWinSequence(_endLevelCutscene);
        }
    }

    // --- NEW: Dynamic Win Trigger ---
    // Allows other scripts (like the Boss) to hand over a specific cutscene to play
    public void TriggerWinSequence(CutsceneTrigger cutsceneToPlay)
    {
        if (_isGameCleared) return; // Prevent it from firing twice
        _isGameCleared = true;
        StartCoroutine(ShowWinSequence(cutsceneToPlay));
    }

    private IEnumerator ShowWinSequence(CutsceneTrigger cutsceneToPlay)
    {
        if (_winPanel != null) _winPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);

        if (_winPanel != null) _winPanel.SetActive(false);

        // Play whichever cutscene was handed to it
        if (cutsceneToPlay != null)
        {
            cutsceneToPlay.PlayFromGameManager();
        }
    }

    // --- ENEMY REGISTRATION ---
    public void RegisterEnemy(GameObject enemy)
    {
        if (!_enemies.Contains(enemy))
        {
            _enemies.Add(enemy);
            _hasStartedTracking = true;
        }
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
        }
    }

    // --- MENUS & TIME MANAGEMENT ---
    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            Time.timeScale = 0f;
            if (_pausePanel != null) _pausePanel.SetActive(true);
        }
        else
        {
            // Only unfreeze time if we aren't currently talking or using the printer!
            if (!NPCDialogue.IsTalking && !PrinterInteractable.IsInteracting)
            {
                Time.timeScale = 1f;
            }
            if (_pausePanel != null) _pausePanel.SetActive(false);
        }
    }

    public void TriggerGameOver()
    {
        _isGameOver = true;
        Time.timeScale = 0f;
        if (_gameOverPanel != null) _gameOverPanel.SetActive(true);
        if (_pausePanel != null) _pausePanel.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ResumeGame()
    {
        _isPaused = false;

        if (!NPCDialogue.IsTalking && !PrinterInteractable.IsInteracting)
        {
            Time.timeScale = 1f;
        }
        if (_pausePanel != null) _pausePanel.SetActive(false);
    }
}