using Farm.Gameplay.Configs;
using Farm.Interface;
using UnityEngine;
using Zenject;

namespace Farm.DI
{
    [CreateAssetMenu(fileName = "Configs Installer", menuName = "Game Resources/Configs Installer")]
    public class ConfigsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private BaitConfig _baitConfig;
        [SerializeField] private CapsuleConfig _capsuleConfig;
        [SerializeField] private InventoryConfig _inventoryConfig;
        [SerializeField] private UpgradeModuleConfig _upgradeModuleConfig;

        public override void InstallBindings()
        {
            Container
                .Bind<BaitConfig>()
                .FromInstance(_baitConfig)
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