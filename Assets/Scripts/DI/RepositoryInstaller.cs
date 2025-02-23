using Farm.Gameplay.Repositories;
using UnityEngine;
using Zenject;

namespace Farm.DI
{
    [CreateAssetMenu(fileName = "Repository Installer", menuName = "Game Resources/Repository Installer")]
    public class RepositoryInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private UpgradeModuleRepository _upgradeModuleRepository;
        [SerializeField] private TheOldOneRepository _theOldOneRepository;

        public override void InstallBindings()
        {
            Container
                .Bind<UpgradeModuleRepository>()
                .FromInstance(_upgradeModuleRepository)
                .AsSingle();
            
            Container
                .Bind<TheOldOneRepository>()
                .FromInstance(_theOldOneRepository)
                .AsSingle();
        }
    }
}
