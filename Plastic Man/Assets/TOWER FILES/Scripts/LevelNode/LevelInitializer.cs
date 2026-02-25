using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    [SerializeField] private string[] nextLevelNames;

    private void Start()
    {
        if (ProgressionManager.Instance != null)
        {
            ProgressionManager.Instance.SetNextLevels(nextLevelNames);
        }
    }
}
