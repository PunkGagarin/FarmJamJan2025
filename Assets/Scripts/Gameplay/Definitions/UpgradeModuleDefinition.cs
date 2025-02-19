using UnityEngine;

namespace Farm.Gameplay.Definitions
{
    [CreateAssetMenu(fileName = "Upgrades Modules Definition", menuName = "Game Resources/Definition/Upgrades Modules")]
    public class UpgradeModuleDefinition: Definition
    {
        [SerializeField] private Sprite _icon;
        
        public Sprite Icon => _icon;
    }
}