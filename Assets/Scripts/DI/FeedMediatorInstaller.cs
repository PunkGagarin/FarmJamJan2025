using Farm.Gameplay;
using Zenject;

namespace Farm.DI
{
    public class FeedMediatorInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<FeedMediator>()
                .FromNew()
                .AsSingle();
        }
    }
}
