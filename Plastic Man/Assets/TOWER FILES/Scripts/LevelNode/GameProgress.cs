using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance;

    public HashSet<string> unlockedLevels = new HashSet<string>();
    public HashSet<string> completedLevels = new HashSet<string>();
    private int currentTier = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);

        unlockedLevels.Add("Level1A");
        unlockedLevels.Add("Level1B");
        unlockedLevels.Add("Level1C");
    }

    public void UnlockLevel(string levelName)
    {
        int newTier = GetLevelTier(levelName);

        if (newTier != currentTier)
        {
            var levelsToLock = unlockedLevels
                .Where(level => GetLevelTier(level) == currentTier)
                .ToList();

            foreach (string level in levelsToLock)
            {
                unlockedLevels.Remove(level);
            }

            currentTier = newTier;
        }

        unlockedLevels.Add(levelName);
    }

    public bool IsUnlocked(string levelName)
    {
        return unlockedLevels.Contains(levelName);
    }

    private int GetLevelTier(string levelName)
    {
        if (levelName.StartsWith("Level"))
        {
            string numberPart = new string(levelName.Substring(5).TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(numberPart, out int tier))
            {
                return tier;
            }
        }
        return -1;
    }

    public void MarkLevelCompleted(string levelName)
    {
        completedLevels.Add(levelName);
    }

    public bool IsCompleted(string levelName)
    {
        return completedLevels.Contains(levelName);
    }
}
