using System;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Gameplay.Configs
{
    [CreateAssetMenu(fileName = "Bait Config", menuName = "Game Resources/Configs/Bait")]
    public class BaitConfig : ScriptableObject
    {
        [SerializeField] private List<BaitTier> _baitTiers;
        
        public List<BaitTier> BaitTiers => _baitTiers;

        [Serializable]
        public class BaitTier
        {
            [SerializeField, Min(1)] private int _baseFoodAmount;
            [SerializeField, Min(1)] private int _baseEnergyAmount;
            [SerializeField, Min(.1f)] private float _baseGrowthSpeed;
            [SerializeField, Min(0)] private int _baseCost;

            public int BaseFoodAmount => _baseFoodAmount;
            public int BaseEnergyAmount => _baseEnergyAmount;
            public int BaseCost => _baseCost;
            public float BaseGrowthSpeed => _baseGrowthSpeed;
        }
    }
}
