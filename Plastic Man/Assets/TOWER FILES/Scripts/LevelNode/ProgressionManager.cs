using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }

    [SerializeField] private string[] nextLevelNames;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetNextLevels(string[] levelNames)
    {
        nextLevelNames = levelNames;
    }

    public void WinLevel()
    {
        string currentLevelName = SceneManager.GetActiveScene().name;

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.MarkLevelCompleted(currentLevelName);

            if (nextLevelNames != null)
            {
                foreach (string level in nextLevelNames)
                {
                    GameProgress.Instance.UnlockLevel(level);
                }
            }
        }

        SceneManager.LoadScene("LevelSelect");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ProgressionManager.Instance.WinLevel();
        }
    }
}
