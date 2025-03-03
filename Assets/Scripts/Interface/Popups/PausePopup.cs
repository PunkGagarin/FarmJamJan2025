using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Interface.Popups
{
    public class PausePopup : Popup
    {
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _optionsButton;
        [SerializeField] private Button _returnButton;

        [Inject] private PopupManager _popupManager;
        [Inject] private SceneLoader.SceneLoader _sceneLoader;

        private void Awake()
        {
            _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            _optionsButton.onClick.AddListener(OnOptionsClicked);
            _returnButton.onClick.AddListener(Close);
        }

        private void OnOptionsClicked() => _popupManager.OpenOptions();

        private void OnMainMenuClicked() => _sceneLoader.Load(SceneLoader.SceneLoader.Scene.MainMenuScene);

        private void OnDestroy()
        {
            _mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
            _optionsButton.onClick.RemoveListener(OnOptionsClicked);
            _returnButton.onClick.RemoveListener(Close);
        }
    }
}