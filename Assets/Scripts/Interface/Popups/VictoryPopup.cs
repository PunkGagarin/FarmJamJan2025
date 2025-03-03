using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Interface.Popups
{
    public class VictoryPopup : Popup
    {
        [SerializeField] private Button _button;

        [Inject] private SceneLoader.SceneLoader _sceneLoader;
        
        private void Awake()
        {
            _button.onClick.AddListener(OpenMainMenu);
        }        
        
        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OpenMainMenu);
        }
        
        private void OpenMainMenu()
        {
            _sceneLoader.Load(SceneLoader.SceneLoader.Scene.MainMenuScene);
        }
    }
}
