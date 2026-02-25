using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelType
{
    Normal,
    Chest,
    Merchant
}

public class LevelNode : MonoBehaviour
{
    public string levelSceneName;
    public bool isUnlocked = false;
    public Sprite completedSprite;
    public GameObject unlockedVFXPrefab;

    public LevelType levelType = LevelType.Normal;
    public string[] nextLevels;
    public static LevelNode LastClickedNode { get; private set; }

    private void Start()
    {
        isUnlocked = GameProgress.Instance != null && GameProgress.Instance.IsUnlocked(levelSceneName);
        bool isCompleted = GameProgress.Instance != null && GameProgress.Instance.IsCompleted(levelSceneName);

        if (isCompleted && completedSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = completedSprite;
        }

        if (isUnlocked && !isCompleted && unlockedVFXPrefab != null)
        {
            Instantiate(unlockedVFXPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    private void OnMouseDown()
    {
        if (!isUnlocked || GameProgress.Instance.IsCompleted(levelSceneName))
            return;

        if (UILevelOverlayManager.Instance != null && UILevelOverlayManager.Instance.IsOverlayActive())
            return;

        LastClickedNode = this;

        if (nextLevels != null && nextLevels.Length > 0)
        {
            ProgressionManager.Instance.SetNextLevels(nextLevels);
        }

        switch (levelType)
        {
            case LevelType.Normal:
                SceneManager.LoadScene(levelSceneName);
                break;
            case LevelType.Chest:
                UILevelOverlayManager.Instance.ShowChest();
                break;
            case LevelType.Merchant:
                UILevelOverlayManager.Instance.ShowMerchant();
                break;
        }
    }
}
