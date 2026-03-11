using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour
{
    [SerializeField] private GameObject _panel;

    [System.Serializable]
    public class SceneButton
    {
        public Button button;
        public string sceneName;
    }

    [SerializeField]
    private SceneButton[] sceneButtons;

    [SerializeField]
    private Button exitButton;

    private void Start()
    {
        foreach (SceneButton sceneButton in sceneButtons)
        {
            if (sceneButton.button != null && !string.IsNullOrEmpty(sceneButton.sceneName))
            {
                sceneButton.button.onClick.AddListener(() => LoadScene(sceneButton.sceneName));
            }
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitApplication);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePanel();
        }
    }

    public void OpenPanelOnPress()
    {
        if (_panel != null)
        {
            _panel.SetActive(true);
            PauseGame();
        }
        else
        {
            Debug.LogWarning("Panel is not assigned in the Inspector!");
        }
    }

    public void ClosePanelOnPress()
    {
        if (_panel != null)
        {
            _panel.SetActive(false);
            ResumeGame();
        }
        else
        {
            Debug.LogWarning("Panel is not assigned in the Inspector!");
        }
    }

    private void TogglePanel()
    {
        if (_panel != null)
        {
            bool isActive = _panel.activeSelf;
            _panel.SetActive(!isActive);

            if (isActive)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        else
        {
            Debug.LogWarning("Panel is not assigned in the Inspector!");
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        AudioListener.pause = true; 
        Debug.Log("Game paused and audio frozen");
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        AudioListener.pause = false; 
        Debug.Log("Game resumed and audio unfrozen");
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void ExitApplication()
    {
        Application.Quit();
        Debug.Log("Application has been exited");
    }
}
