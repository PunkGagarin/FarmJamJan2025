using System.Collections.Generic;
using Farm.Gameplay.Configs.UpgradeModules;
using UnityEngine;

public class UpgradeModule
{
    private List<UpgradeModuleStat> _stats;
    public Sprite Icon { get; set; }

    public List<UpgradeModuleStat> Stats => _stats;

    public UpgradeModule(List<UpgradeModuleStat> stats, Sprite icon)
    {
        _stats = stats;
        Icon = icon;
    }
}