using UnityEngine;
using Zenject;
namespace Farm.SceneLoader
{
    public class SceneLoaderCallback : MonoBehaviour
    {
        private bool _isFirstUpdate = true;

        [Inject] private SceneLoader _loader;

        private void Update()
        {
            if (!_isFirstUpdate) return;
            _isFirstUpdate = false;
            _loader.LoaderCallback();
        }
    }
}