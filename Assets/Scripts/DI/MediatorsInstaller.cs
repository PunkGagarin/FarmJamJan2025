using Farm.Gameplay;
using Zenject;

namespace Farm.DI
{
    public class MediatorsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<FeedMediator>()
                .FromNew()
                .AsSingle();
            
            Container
                .Bind<MiniGameEffectsMediator>()
                .FromNew()
                .AsSingle();
        }
    }
}
