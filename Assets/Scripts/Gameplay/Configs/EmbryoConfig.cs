using System;
using System.Collections.Generic;
using Farm.Enums;
using UnityEngine;

namespace Farm.Gameplay.Configs
{
    [CreateAssetMenu(fileName = "Bait Config", menuName = "Game Resources/Configs/Bait")]
    public class EmbryoConfig : ScriptableObject
    {
        [SerializeField] private List<EmbryoTier> _embryoTiers;
        [SerializeField] private Sprite _humanImage, _animalImage, _fishImage;
        public Sprite HumanImage => _humanImage;
        public Sprite AnimalImage => _animalImage;
        public Sprite FishImage => _fishImage;

        public List<EmbryoTier> EmbryoTiers => _embryoTiers;

        public Sprite GetSprite(EmbryoType embryoType) => embryoType switch
        {
            EmbryoType.Human => _humanImage,
            EmbryoType.Animal => _animalImage,
            EmbryoType.Fish => _fishImage,
            _ => null
        };

        [Serializable]
        public class EmbryoTier
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
