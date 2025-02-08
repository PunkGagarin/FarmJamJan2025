using System.Runtime.InteropServices;
using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenuUi : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;

    [SerializeField] private OptionsDialog optionsDialog;
    [SerializeField] private CreditsDialog creditsDialog;

    [Inject] private MusicManager musicManager;
    
    private void Awake()
    {
        startGameButton.onClick.AddListener(OnStartGameClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
        musicManager.PlaySoundByType(GameAudioType.MainMenuBgm, 0);
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