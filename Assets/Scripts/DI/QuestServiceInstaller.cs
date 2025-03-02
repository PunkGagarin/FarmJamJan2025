using Farm.Gameplay.Quests;
using Zenject;

namespace Farm.DI
{
    public class QuestServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<QuestService>().AsSingle();
        }
    }
}
