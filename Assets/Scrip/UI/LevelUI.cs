// 修改后的LevelUI.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private Button nextButton;

    void Start()
    {
        int maxLevel = GameManager.Instance.GetMaxUnlockedLevel();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 设置关卡按钮状态
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int buttonLevel = i + 1;
            levelButtons[i].interactable = (buttonLevel <= maxLevel);
        }

        // 新逻辑：只要当前关卡不是最后一关就可以按下一关
        nextButton.interactable = (currentSceneIndex < 3); // 3是总关卡数
    }

    public void OnMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    public void OnLevelSelect(int levelNumber)
    {
        GameManager.Instance.LoadLevel(levelNumber);
    }

    public void OnNextLevel()
    {
        GameManager.Instance.GoToNextLevel();
    }
}