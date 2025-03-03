using Farm.Audio;
using Farm.Interface.Popups;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace Farm.Interface
{
    public class MainMenuUi : MonoBehaviour
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _optionsButton;
        [SerializeField] private Button _creditsButton;

        [Inject] private MusicManager _musicManager;
        [Inject] private SceneLoader.SceneLoader _sceneLoader;
        [Inject] private PopupManager _popupManager;
    
        private void Awake()
        {
            _startGameButton.onClick.AddListener(OnStartGameClicked);
            _optionsButton.onClick.AddListener(OnOptionsClicked);
            _creditsButton.onClick.AddListener(OnCreditsClicked);
        }

        private void Start()
        {
            _musicManager.PlayBgm();
        }

        private void OnStartGameClicked()
        {
            _sceneLoader.Load(SceneLoader.SceneLoader.Scene.Gameplay);
        }

        private void OnOptionsClicked()
        {
            _popupManager.OpenOptions();
        }

        private void OnCreditsClicked()
        {
            _popupManager.OpenCredits();
        }

        private void OnDestroy()
        {
            _startGameButton.onClick.AddListener(OnStartGameClicked);
            _optionsButton.onClick.AddListener(OnOptionsClicked);
            _creditsButton.onClick.AddListener(OnCreditsClicked);
        }
    }
}