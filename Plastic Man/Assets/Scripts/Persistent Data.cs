using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance { get; private set; }

    [Header("Scene Load")]
    [SerializeField] private string nextScene;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene);
    }
}
