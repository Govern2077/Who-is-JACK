using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI: MonoBehaviour
{
    [SerializeField] private Button continueButton;

    void Start()
    {
        // ���û�д浵������Continue��ť
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