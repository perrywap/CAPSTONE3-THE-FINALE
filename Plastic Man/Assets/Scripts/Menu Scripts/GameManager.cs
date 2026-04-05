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

    [Header("Enemy Tracker")]
    [SerializeField] private List<GameObject> _enemies = new List<GameObject>();

    [Header("End Level Cutscene")]
    [Tooltip("Drag your final CutsceneTrigger here. It will play immediately after the Win Panel finishes.")]
    [SerializeField] private CutsceneTrigger _endLevelCutscene;

    private bool _isPaused = false;
    public bool IsPaused { get { return _isPaused; } }
    private bool _isGameOver = false;
    private bool _isGameCleared = false;

    public bool IsGameCleared { get { return _isGameCleared; } }

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (_isGameCleared)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) && !_isGameOver)
        {
            TogglePause();
        }

        // Clean up the list to prevent errors if an enemy gets destroyed unexpectedly
        _enemies.RemoveAll(item => item == null);

        // Win Condition: All registered enemies are gone
        if (_enemies.Count <= 0)
        {
            _isGameCleared = true;
            StartCoroutine(ShowWinSequence());
        }
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (!_enemies.Contains(enemy))
        {
            _enemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
        }
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
            if (!NPCDialogue.IsTalking)
            {
                Time.timeScale = 1f;
            }

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

        if (!NPCDialogue.IsTalking)
        {
            Time.timeScale = 1f;
        }

        _pausePanel.SetActive(false);
    }

    private IEnumerator ShowWinSequence()
    {
        _winPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(3f); // Using Realtime so it works if timescale is altered

        _winPanel.SetActive(false);

        if (_endLevelCutscene != null)
        {
            _endLevelCutscene.PlayFromGameManager();
        }
    }
}