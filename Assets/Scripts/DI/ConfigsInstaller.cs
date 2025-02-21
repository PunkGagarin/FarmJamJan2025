using Farm.Gameplay.Configs;
using UnityEngine;
using Zenject;

namespace Farm.DI
{
    [CreateAssetMenu(fileName = "Configs Installer", menuName = "Game Resources/Configs Installer")]
    public class ConfigsInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private BaitConfig _baitConfig;
        [SerializeField] private CapsuleConfig _capsuleConfig;
        
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
        }
    }
}
