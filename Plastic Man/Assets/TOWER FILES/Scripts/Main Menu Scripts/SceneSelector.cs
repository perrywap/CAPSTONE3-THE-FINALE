using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour
{
    [SerializeField] private GameObject _panel;

    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioClip _buttonClickSound;
    [SerializeField] private AudioClip _panelOpenSound;
    [SerializeField] private AudioClip _panelCloseSound;

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
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                Debug.LogWarning("No AudioSource assigned or found on this GameObject. Sound effects will not play.");
            }
        }

        foreach (SceneButton sceneButton in sceneButtons)
        {
            if (sceneButton.button != null && !string.IsNullOrEmpty(sceneButton.sceneName))
            {
                sceneButton.button.onClick.AddListener(() =>
                {
                    PlaySound(_buttonClickSound);
                    LoadScene(sceneButton.sceneName);
                });
            }
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(() =>
            {
                PlaySound(_buttonClickSound);
                ExitApplication();
            });
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
            PlaySound(_panelOpenSound);
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
            PlaySound(_panelCloseSound);
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
                PlaySound(_panelCloseSound);
            }
            else
            {
                PauseGame();
                PlaySound(_panelOpenSound);
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

    private void PlaySound(AudioClip clip)
    {
        if (_audioSource != null && clip != null)
        {
            _audioSource.PlayOneShot(clip);
        }
    }
}