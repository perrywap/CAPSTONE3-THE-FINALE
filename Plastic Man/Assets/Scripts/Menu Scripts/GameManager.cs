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
    [SerializeField] private RoguePrinterSpawner _levelBoss;

    [Header("Enemy Tracker")]
    [SerializeField] private List<GameObject> _enemies = new List<GameObject>();

    [Header("End Level Cutscene")]
    [SerializeField] private CutsceneTrigger _endLevelCutscene;

    private bool _isPaused = false;
    public bool IsPaused { get { return _isPaused; } }
    private bool _isGameOver = false;
    private bool _isGameCleared = false;
    public bool IsGameCleared { get { return _isGameCleared; } }

    private bool _hasStartedTracking = false;
    public int ActiveEnemyCount => _enemies.Count;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (_enemies.Count > 0) _hasStartedTracking = true;
    }

    void Update()
    {
        if (_isGameCleared) return;

        if (Input.GetKeyDown(KeyCode.Escape) && !_isGameOver)
        {
            if (PrinterInteractable.IsInteracting)
            {
                PrinterInteractable printer = Object.FindFirstObjectByType<PrinterInteractable>();
                if (printer != null) printer.ClosePrinter();
            }
            else if (!NPCDialogue.IsTalking)
            {
                TogglePause();
            }
        }

        _enemies.RemoveAll(item => item == null);

        bool bossDefeated = _levelBoss == null || _levelBoss.isDefeated;

        if (_hasStartedTracking && _enemies.Count <= 0 && bossDefeated)
        {
            TriggerWinSequence(_endLevelCutscene);
        }
    }

    public void TriggerWinSequence(CutsceneTrigger cutsceneToPlay)
    {
        if (_isGameCleared) return;
        _isGameCleared = true;
        StartCoroutine(ShowWinSequence(cutsceneToPlay));
    }

    private IEnumerator ShowWinSequence(CutsceneTrigger cutsceneToPlay)
    {
        _winPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        _winPanel.SetActive(false);

        if (cutsceneToPlay != null) cutsceneToPlay.PlayFromGameManager();
    }

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
        if (_enemies.Contains(enemy)) _enemies.Remove(enemy);
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            Time.timeScale = 0f;
            _pausePanel.SetActive(true);
        }
        else
        {
            if (!NPCDialogue.IsTalking && !PrinterInteractable.IsInteracting) Time.timeScale = 1f;
            _pausePanel.SetActive(false);
        }
    }

    public void TriggerGameOver()
    {
        _isGameOver = true;
        Time.timeScale = 0f;
        _gameOverPanel.SetActive(true);
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
        if (!NPCDialogue.IsTalking && !PrinterInteractable.IsInteracting) Time.timeScale = 1f;
        _pausePanel.SetActive(false);
    }
}