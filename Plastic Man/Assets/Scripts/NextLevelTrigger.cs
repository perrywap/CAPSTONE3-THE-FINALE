using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTrigger : MonoBehaviour
{
    [SerializeField] private string nextScene;
    private BoxCollider2D collider;
    public bool isLocked;

    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = false;
    }

    private void Update()
    {
        if(GameManager.Instance.IsGameCleared)
            collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if(player != null )
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }
}
