using NUnit.Framework;
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

   

    private bool _isPaused = false;
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

        if (_enemies.Count <= 0)
        {
            _isGameCleared = true;
            StartCoroutine(ShowWinPanel());
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
            Time.timeScale = 1f;
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
        Time.timeScale = 1f;
        _pausePanel.SetActive(false);
    }

    private IEnumerator ShowWinPanel()
    {
        _winPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        _winPanel.SetActive(false);
    }
}