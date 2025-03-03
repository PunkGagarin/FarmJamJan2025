using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Interface.Popups
{
    public class GameOverPopup : Popup
    {
        [SerializeField] private Button _button;
        
        [Inject] private SceneLoader.SceneLoader _sceneLoader;

        private void Awake()
        {
            _button.onClick.AddListener(RestartScene);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void RestartScene()
        {
            _sceneLoader.Load(SceneLoader.SceneLoader.Scene.Gameplay);
        }
    }
}
