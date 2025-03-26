// �޸ĺ��LevelUI.cs
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

        // ���ùؿ���ť״̬
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int buttonLevel = i + 1;
            levelButtons[i].interactable = (buttonLevel <= maxLevel);
        }

        // ���߼���ֻҪ��ǰ�ؿ��������һ�ؾͿ��԰���һ��
        nextButton.interactable = (currentSceneIndex < 3); // 3���ܹؿ���
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