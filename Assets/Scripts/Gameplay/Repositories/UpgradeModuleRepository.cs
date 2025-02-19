using Farm.Gameplay.Definitions;
using UnityEngine;

namespace Farm.Gameplay.Repositories
{
    [CreateAssetMenu(fileName = "Upgrades Modules Repository", menuName = "Game Resources/Repository/Upgrades Modules")]
    public class UpgradeModuleRepository: Repository<UpgradeModuleDefinition>
    {
        public UpgradeModule GetRandomModule()
        {
            var index = Random.Range(0, Definitions.Count);
            return new UpgradeModule(Definitions[index]);
        }
    }
}