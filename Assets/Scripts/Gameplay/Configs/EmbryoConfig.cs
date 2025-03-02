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
        [SerializeField] private Sprite _humanImageUI, _animalImageUI, _fishImageUI, _humanImageUIEnd, _animalImageUIEnd, _fishImageUIEnd, _humanImageStart, _animalImageStart, _fishImageStart, _humanImageEnd, _animalImageEnd, _fishImageEnd;
        
        public List<EmbryoTier> EmbryoTiers => _embryoTiers;

        public Sprite GetUISprite(EmbryoType embryoType) => embryoType switch
        {
            EmbryoType.Human => _humanImageUI,
            EmbryoType.Animal => _animalImageUI,
            EmbryoType.Fish => _fishImageUI,
            _ => null
        };
        
        public Sprite GetUISpriteEnd(EmbryoType embryoType) => embryoType switch
        {
            EmbryoType.Human => _humanImageUIEnd,
            EmbryoType.Animal => _animalImageUIEnd,
            EmbryoType.Fish => _fishImageUIEnd,
            _ => null
        };
        
        public Sprite GetSpriteStart(EmbryoType embryoType) => embryoType switch
        {
            EmbryoType.Human => _humanImageStart,
            EmbryoType.Animal => _animalImageStart,
            EmbryoType.Fish => _fishImageStart,
            _ => null
        };
        
        public Sprite GetSpriteEnd(EmbryoType embryoType) => embryoType switch
        {
            EmbryoType.Human => _humanImageEnd,
            EmbryoType.Animal => _animalImageEnd,
            EmbryoType.Fish => _fishImageEnd,
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
