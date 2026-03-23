using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _gameOverPanel;

    private bool _isPaused = false;
    private bool _isGameOver = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_isGameOver)
        {
            TogglePause();
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
}