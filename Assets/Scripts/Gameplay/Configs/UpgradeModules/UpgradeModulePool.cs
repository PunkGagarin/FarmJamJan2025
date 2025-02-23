using System.Collections.Generic;
using UnityEngine;

namespace Farm.Gameplay.Configs.UpgradeModules
{
    [CreateAssetMenu(fileName = "Upgrade Module Pool", menuName = "Game Resources/Configs/Upgrade Module Pool")]
    public class UpgradeModulePool: ScriptableObject
    {
        [SerializeField] private List<UpgradeModuleStat> stats;
        public List<UpgradeModuleStat> Stats => stats;
    }
}