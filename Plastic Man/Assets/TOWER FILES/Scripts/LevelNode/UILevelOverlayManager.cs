using UnityEngine;

public class UILevelOverlayManager : MonoBehaviour
{
    public static UILevelOverlayManager Instance;

    [Header("Panels")]
    public GameObject chestPanel;
    public GameObject merchantPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        chestPanel.SetActive(false);
        merchantPanel.SetActive(false);
    }

    public void ShowChest()
    {
        chestPanel.SetActive(true);
    }

    public void ShowMerchant()
    {
        merchantPanel.SetActive(true);
    }

    public bool IsOverlayActive()
    {
        return chestPanel.activeSelf || merchantPanel.activeSelf;
    }

    public void Hide()
    {
        chestPanel.SetActive(false);
        merchantPanel.SetActive(false);
    }

    public void OnLeaveButtonClicked()
    {
        if (LevelNode.LastClickedNode != null)
        {
            var levelName = LevelNode.LastClickedNode.levelSceneName;

            GameProgress.Instance.MarkLevelCompleted(levelName);

            if (LevelNode.LastClickedNode.nextLevels != null)
            {
                foreach (var level in LevelNode.LastClickedNode.nextLevels)
                {
                    GameProgress.Instance.UnlockLevel(level);
                }
            }

            LevelNode[] allNodes = FindObjectsOfType<LevelNode>();
            foreach (var node in allNodes)
            {
                node.isUnlocked = GameProgress.Instance.IsUnlocked(node.levelSceneName);
                bool isCompleted = GameProgress.Instance.IsCompleted(node.levelSceneName);

                if (isCompleted && node.completedSprite != null)
                {
                    node.GetComponent<SpriteRenderer>().sprite = node.completedSprite;
                }

                foreach (Transform child in node.transform)
                {
                    if (child.GetComponent<ParticleSystem>())
                    {
                        Destroy(child.gameObject);
                    }
                }

                if (node.isUnlocked && !isCompleted && node.unlockedVFXPrefab != null)
                {
                    Instantiate(node.unlockedVFXPrefab, node.transform.position, Quaternion.identity, node.transform);
                }
            }
        }

        Hide();
    }
}
