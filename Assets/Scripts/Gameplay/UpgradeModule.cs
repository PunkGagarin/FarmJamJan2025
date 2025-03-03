using System.Collections.Generic;
using Farm.Gameplay.Configs.UpgradeModules;

namespace Farm.Gameplay
{
    public class UpgradeModule
    {
        private List<UpgradeModuleStat> _stats;

        public List<UpgradeModuleStat> Stats => _stats;

        public UpgradeModule(List<UpgradeModuleStat> stats)
        {
            _stats = stats;
        }
    }
}