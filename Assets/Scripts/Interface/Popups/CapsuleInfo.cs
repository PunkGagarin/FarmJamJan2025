using System;
using Farm.Gameplay.Capsules;
using TMPro;
using UnityEngine;

namespace Farm.Interface.Popups
{
    [Serializable]
    public class CapsuleInfo
    {
        [SerializeField] private TMP_Text _capsuleTier;
        [SerializeField] private TMP_Text _humanChance;
        [SerializeField] private TMP_Text _animalChance;
        [SerializeField] private TMP_Text _fishChance;
        [SerializeField] private TMP_Text _foodAmount;
        [SerializeField] private TMP_Text _energyAmount;
        [SerializeField] private TMP_Text _growTime;
        [SerializeField] private GameObject _statsPanel;

        private const string CAPSULE_TIER = "Capsule Tier ";
        private const float PERCENT_VALUE = 100f;
        
        public void SetCapsuleInfo(int tier, float chanceHuman, float chanceAnimal, float chanceFish)
        {
            _capsuleTier.text = $"{CAPSULE_TIER}{tier}";
            float totalChance = chanceAnimal+chanceHuman+chanceFish;
            _humanChance.text = (chanceHuman / totalChance * PERCENT_VALUE).ToString("0") + "%";
            _animalChance.text = (chanceAnimal / totalChance * PERCENT_VALUE).ToString("0") + "%";
            _fishChance.text = (chanceFish / totalChance * PERCENT_VALUE).ToString("0") + "%";
        }

        public void SetEmbryoInfo(Embryo embryo)
        {
            if (embryo == null)
            {
                _statsPanel.SetActive(false);
            }
            else
            {
                _statsPanel.SetActive(true);
                _foodAmount.text = embryo.StarvationValue.ToString();
                _energyAmount.text = embryo.EnergyValue.ToString();
                _growTime.text = embryo.TimeToGrowth.ToString("0.0");
            }
        }
    }
}