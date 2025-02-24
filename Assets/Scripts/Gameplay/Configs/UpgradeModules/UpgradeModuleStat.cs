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
        public float MinValue;
        public float MaxValue;

        public float Value { get; private set; }

        public void SetValue(float value) => Value = value;

        public override string ToString()
        {
            return $"ModuleCharacteristicType: {ModuleCharacteristicType}, Value: {Value}";
        }
    }
}