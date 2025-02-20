using Farm.Gameplay.Definitions;
using UnityEngine;

public class UpgradeModule
{
    public UpgradeModuleDefinition Definition;

    public Sprite Icon => Definition.Icon;
    
    public UpgradeModule(UpgradeModuleDefinition definition)
    {
        Definition = definition;
    }
}