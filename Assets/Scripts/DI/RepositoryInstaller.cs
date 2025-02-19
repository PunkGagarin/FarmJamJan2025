using Farm.Gameplay.Repositories;
using UnityEngine;
using Zenject;

namespace Farm.DI
{
    [CreateAssetMenu(fileName = "Repository Installer", menuName = "Game Resources/Repository Installer")]
    public class RepositoryInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private EmbryoRepository _embryoRepository;
        [SerializeField] private UpgradeModuleRepository _upgradeModuleRepository;

        public override void InstallBindings()
        {
            Container
                .Bind<EmbryoRepository>()
                .FromInstance(_embryoRepository)
                .AsSingle();
            
            Container
                .Bind<UpgradeModuleRepository>()
                .FromInstance(_upgradeModuleRepository)
                .AsSingle();
        }
    }
}
