using Farm.Utils.Timer;
using Zenject;

namespace Farm.DI
{
    public class TimerServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<TimerService>()
                .AsSingle()
                .NonLazy();
        }
    }
}
