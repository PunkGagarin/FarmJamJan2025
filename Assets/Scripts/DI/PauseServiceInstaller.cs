using Farm.Utils.Pause;
using Zenject;

namespace Farm.DI
{
    public class PauseServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<PauseService>()
                .AsSingle()
                .NonLazy();
        }
    }
}
