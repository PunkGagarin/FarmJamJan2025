using Zenject;
namespace Farm.DI
{
    public class LoaderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SceneLoader.SceneLoader>().AsSingle();
        }
    }
}