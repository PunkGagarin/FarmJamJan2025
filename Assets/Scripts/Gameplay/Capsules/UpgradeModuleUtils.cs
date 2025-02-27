using System.Collections.Generic;
using System.Linq;
using Farm.Gameplay.Capsules;
using Farm.Gameplay.Configs.UpgradeModules;
using UnityEngine;

public class UpgradeModuleUtils
{
    public static float ApplyStatsWithType(float baseChance, List<CapsuleSlot> slots,
        ModuleCharacteristicType moduleCharacteristicType)
    {
        var chance = baseChance;
        slots.ForEach(capsuleSlot =>
        {
            if (capsuleSlot.UpgradeModule == null) return;
            var rollStat =
                capsuleSlot.UpgradeModule.Stats.FirstOrDefault(stat =>
                    stat.ModuleCharacteristicType == moduleCharacteristicType);
            if (rollStat != null) chance *= 1 + rollStat.Value / 100f;
        });
        return Mathf.Min(chance, 100);
    }
}