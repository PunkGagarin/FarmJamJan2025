using Farm.Gameplay.Configs;
using Farm.Gameplay.Configs.UpgradeModules;
using Farm.Interface;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Farm.DI
{
    [CreateAssetMenu(fileName = "Configs Installer", menuName = "Game Resources/Configs Installer")]
    public class ConfigsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private EmbryoConfig _embryoConfig;
        [SerializeField] private CapsuleConfig _capsuleConfig;
        [SerializeField] private InventoryConfig _inventoryConfig;
        [SerializeField] private UpgradeModuleConfig _upgradeModuleConfig;

        public override void InstallBindings()
        {
            Container
                .Bind<EmbryoConfig>()
                .FromInstance(_embryoConfig)
                .AsSingle();

            Container
                .Bind<CapsuleConfig>()
                .FromInstance(_capsuleConfig)
                .AsSingle();
            
            Container
                .Bind<InventoryConfig>()
                .FromInstance(_inventoryConfig)
                .AsSingle();
            
            Container
                .Bind<UpgradeModuleConfig>()
                .FromInstance(_upgradeModuleConfig)
                .AsSingle();
        }
    }
}