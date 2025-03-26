using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI: MonoBehaviour
{
    [SerializeField] private Button continueButton;

    void Start()
    {
        // 如果没有存档，禁用Continue按钮
        continueButton.interactable = PlayerPrefs.HasKey("GameSave");
    }

    public void OnStartButton()
    {
        GameManager.Instance.StartNewGame();
    }

    public void OnContinueButton()
    {
        GameManager.Instance.ContinueGame();
    }

    public void OnQuitButton()
    {
        GameManager.Instance.QuitGame();
    }
}