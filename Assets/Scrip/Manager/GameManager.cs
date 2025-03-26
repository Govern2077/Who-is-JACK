// 修改后的GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 当前所在关卡（用于Continue）
    private int currentLevel = 1;
    // 历史最高关卡（用于关卡选择）
    private int maxUnlockedLevel = 1;
    private const string saveKey = "GameSave";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGame()
    {
        currentLevel = 1;
        maxUnlockedLevel = 1;
        SaveGame();
        LoadScene("Level1");
    }

    public void ContinueGame()
    {
        LoadScene("Level" + currentLevel);
    }

    public void LoadLevel(int targetLevel)
    {
        if (targetLevel <= maxUnlockedLevel)
        {
            currentLevel = targetLevel;
            LoadScene("Level" + targetLevel);
        }
    }
    public int GetSceneIndex(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            if (path.Contains(sceneName))
            {
                return i;
            }
        }
        return -1;
    }

    public void GoToNextLevel()
    {
        int nextLevel = currentLevel + 1;
        if (nextLevel > 3) return;

        // 更新最高解锁进度（重要修改点）
        maxUnlockedLevel = Mathf.Max(maxUnlockedLevel, nextLevel);
        currentLevel = nextLevel;
        SaveGame();
        LoadScene("Level" + nextLevel);
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReturnToMainMenu()
    {
        SaveGame();
        SceneManager.LoadScene("StartMenu");
    }

    private void SaveGame()
    {
        PlayerPrefs.SetInt(saveKey + "_current", currentLevel);
        PlayerPrefs.SetInt(saveKey + "_max", maxUnlockedLevel);
        PlayerPrefs.Save();
    }

    private void LoadGame()
    {
        currentLevel = PlayerPrefs.GetInt(saveKey + "_current", 1);
        maxUnlockedLevel = PlayerPrefs.GetInt(saveKey + "_max", 1);
    }

    public int GetMaxUnlockedLevel()
    {
        return maxUnlockedLevel;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    public void QuitGame()
    {
        SaveGame();
        Application.Quit();
    }
}