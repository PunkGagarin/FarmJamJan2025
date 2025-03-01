using System;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Gameplay.Configs.MiniGame
{
    [CreateAssetMenu(fileName = "Mini game Config", menuName = "Game Resources/Configs/Mini game")]
    public class MiniGameConfig : ScriptableObject
    {
        [SerializeField] private MiniGamePool _pool;
        [SerializeField, Range(5, 60)] private float _tier1Angle;
        [SerializeField, Range(5, 60)] private float _tier2Angle;
        [SerializeField, Range(5, 60)] private float _tier3Angle;
        [SerializeField] private float _decelerationTime;
        [SerializeField] private float _effectTime;
        [SerializeField] private MiniGameRisk _lowRiskStats;
        [SerializeField] private MiniGameRisk _mediumRiskStats;
        [SerializeField] private MiniGameRisk _highRiskStats;
        [SerializeField] private float _additionalAngle;

        public List<MiniGameEffect> Effects => _pool.Effects;
        public float Tier1Angle => _tier1Angle;
        public float Tier2Angle => _tier2Angle;
        public float Tier3Angle => _tier3Angle;
        public float DecelerationTime => _decelerationTime;
        public MiniGameRisk LowRiskStats => _lowRiskStats;
        public MiniGameRisk MediumRiskStats => _mediumRiskStats;
        public MiniGameRisk HighRiskStats => _highRiskStats;
        public float EffectTime => _effectTime;
        public float AdditionalAngle => _additionalAngle;
        public MiniGameEffect FirstEffect => _pool.FirstEffect;
    }
    
    [Serializable]
    public class MiniGameRisk
    {
        public float Speed;
        public int SegmentsOnWheel;
        public int HighestAllowedTier;
        public int CostToRun;
    }
}
