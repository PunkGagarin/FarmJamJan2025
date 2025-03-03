using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm
{
    public class RestartCurrentSceneButton : MonoBehaviour
    {
        private Button _button;
        
        [Inject] private SceneLoader _sceneLoader;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(RestartScene);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void RestartScene()
        {
            _sceneLoader.Load(SceneLoader.Scene.Gameplay);
        }
    }
}