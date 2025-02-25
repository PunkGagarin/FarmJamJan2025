using Farm.Gameplay.Quests;
using Zenject;

namespace Farm.DI
{
    public class QuestProviderInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<QuestProvider>().AsSingle();
        }
    }
}
