using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenuUi : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _creditsButton;

    [SerializeField] private OptionsDialog _optionsDialog;
    [SerializeField] private CreditsDialog _creditsDialog;

    [Inject] private MusicManager _musicManager;
    [Inject] private SceneLoader _sceneLoader;
    
    private void Awake()
    {
        _startGameButton.onClick.AddListener(OnStartGameClicked);
        _optionsButton.onClick.AddListener(OnOptionsClicked);
        _creditsButton.onClick.AddListener(OnCreditsClicked);
        _musicManager.PlaySoundByType(GameAudioType.MainMenuBgm, 0);
    }

    private void OnStartGameClicked()
    {
        _sceneLoader.Load(SceneLoader.Scene.GamePlayScene);
    }

    private void OnOptionsClicked()
    {
        _optionsDialog.Show();
    }

    private void OnCreditsClicked()
    {
        _creditsDialog.Show();
    }

    private void OnDestroy()
    {
        _startGameButton.onClick.AddListener(OnStartGameClicked);
        _optionsButton.onClick.AddListener(OnOptionsClicked);
        _creditsButton.onClick.AddListener(OnCreditsClicked);
    }
}