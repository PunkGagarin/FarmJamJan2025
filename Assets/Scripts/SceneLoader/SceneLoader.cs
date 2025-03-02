using UnityEngine.SceneManagement;
namespace Farm.SceneLoader
{
    public class SceneLoader
    {
        private static Scene _targetScene;

        public enum Scene
        {
            MainMenuScene,
            Gameplay,
            LoadingScene
        }

        public void Load(Scene targetScene)
        {
            _targetScene = targetScene;
            SceneManager.LoadScene(Scene.LoadingScene.ToString());
        }

        public void LoaderCallback()
        {
            SceneManager.LoadScene(_targetScene.ToString());
        }
    }
}