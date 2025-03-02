using System;
using UnityEngine;

namespace Farm.Gameplay.Configs.UpgradeModules
{
    [Serializable]
    public class UpgradeModuleStat
    {
        public ModuleCharacteristicType ModuleCharacteristicType;
        public String Description;
        public Sprite Icon;
        public int MinValue;
        public int MaxValue;

        public int Value { get; private set; }

        public void SetValue(int value) => Value = value;

        public override string ToString()
        {
            return $"ModuleCharacteristicType: {ModuleCharacteristicType}, Value: {Value}";
        }

        public UpgradeModuleStat(ModuleCharacteristicType moduleCharacteristicType, string description, Sprite icon, int minValue, int maxValue)
        {
            ModuleCharacteristicType = moduleCharacteristicType;
            Description = description;
            Icon = icon;
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}