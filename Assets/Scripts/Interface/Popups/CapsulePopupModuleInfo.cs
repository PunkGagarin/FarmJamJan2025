using System;
using System.Collections.Generic;
using System.Linq;
using Farm.Gameplay.Configs.UpgradeModules;
using TMPro;
using UnityEngine;

namespace Farm.Interface.Popups
{
    public class CapsulePopupModuleInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text _humanChance;
        [SerializeField] private TMP_Text _animalChance;
        [SerializeField] private TMP_Text _fishChance;
        [SerializeField] private TMP_Text _growTime;
        [SerializeField] private TMP_Text _energyCharge;
        [SerializeField] private TMP_Text _satiety;

        public void SetInfo(List<UpgradeModuleStat> stats)
        {
            var humanChanceValue = CalculateChanceByType(stats, ModuleCharacteristicType.HumanRoll);
            var animalChanceValue = CalculateChanceByType(stats, ModuleCharacteristicType.AnimalRoll);
            var fishChanceValue = CalculateChanceByType(stats, ModuleCharacteristicType.FishRoll);
            var growthTimeValue = CalculateChanceByType(stats, ModuleCharacteristicType.GrowthSpeed);
            var energyChargeValue = CalculateChanceByType(stats, ModuleCharacteristicType.EnergyCharge);
            var satietyValue = CalculateChanceByType(stats, ModuleCharacteristicType.Satiety);


            ShowValue(_humanChance, humanChanceValue);
            ShowValue(_animalChance, animalChanceValue);
            ShowValue(_fishChance, fishChanceValue);
            ShowValue(_growTime, growthTimeValue);
            ShowValue(_energyCharge, energyChargeValue);
            ShowValue(_satiety, satietyValue);
        }

        private void ShowValue(TMP_Text textView, float value)
        {
            var sign = value >= 0 ? "+" : "-";
            var valueString = $"{sign} {Mathf.Abs(value)}%";
            var color = value >= 0 ? new Color(0, 0.3f, 0, 1) : new Color(0.3f, 0, 0, 1);
            textView.text = valueString;
            textView.color = color;
            textView.rectTransform.parent.gameObject.SetActive(value != 0);
        }

        private float CalculateChanceByType(List<UpgradeModuleStat> stats, ModuleCharacteristicType characteristicType)
        {
            var chance = stats.Where(stat => stat.ModuleCharacteristicType == characteristicType).ToList();
            var first = chance.Count >= 1 ? Mathf.RoundToInt(chance[0].Value) : 0;
            var second = chance.Count >= 2 ? Mathf.RoundToInt(chance[1].Value) : 0;
            return CalculateTotal(first, second);
        }

        private float CalculateTotal(float firstChance, float secondChance)
        {
            var totalIncrease = (1 + firstChance / 100f) * (1 + secondChance / 100f) - 1;
            return MathF.Round(totalIncrease * 100f);
        }
    }
}