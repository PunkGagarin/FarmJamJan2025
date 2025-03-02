using UnityEngine;

namespace Farm.Gameplay.Quests
{
    public enum RequirementType
    {
        [Tooltip("Нужно скормить RequiredAmount людей")]
        Human = 0,
        [Tooltip("Нужно скормить RequiredAmount животных")]
        Animal = 1,
        [Tooltip("Нужно скормить RequiredAmount рыб")]
        Fish = 2,
        [Tooltip("Нужно накопить RequiredAmount энергии")]
        Energy = 3,
        [Tooltip("Нужно поднять сытость до RequiredAmount")]
        Satiety = 4,
        [Tooltip("Нужно открыть RequiredAmount капсул")]
        AvailableCapsules = 5,
        [Tooltip("Нужно улучшить RequiredAmount капсул до уровня RequiredExtraAmount (от 0 до 3)")]
        CapsulesWithTierUpgrade = 6,
        [Tooltip("Нужно установить RequiredAmount модулей")]
        EquippedModules = 7,
        [Tooltip("Нужно продержать капсулы пустыми RequiredAmount времени")]
        EmptyCapsules = 8,
        [Tooltip("Нужно скормить RequiredAmount юнитов в RequiredExtraAmount секунд")]
        CollectUnitsInSeconds = 9,
        [Tooltip("Нужно победить в третьем уровне мини-игры RequiredAmount раз")]
        WinInThirdTierMiniGameTimes = 10,
        [Tooltip("Продержать древнего в режиме ярости RequiredAmount секунд")]
        KeepRampageMode = 11,
    }
}