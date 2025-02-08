using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUi : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;

    [SerializeField] private OptionsDialog optionsDialog;
    [SerializeField] private CreditsDialog creditsDialog;

    private void Awake()
    {
        startGameButton.onClick.AddListener(OnStartGameClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
    }

    private void OnStartGameClicked()
    {
        SceneManager.LoadScene("GamePlayScene");
    }

    private void OnOptionsClicked()
    {
        optionsDialog.Show();
    }

    private void OnCreditsClicked()
    {
        creditsDialog.Show();
    }

    private void OnDestroy()
    {
        startGameButton.onClick.AddListener(OnStartGameClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
    }
}