using System;
using UnityEngine;

namespace Farm.Gameplay.MiniGame
{
    [CreateAssetMenu(fileName = "Mock Mini Game Info", menuName = "Game Resources/Mock Mini Game Info")]
    public class MockMiniGameInfo : ScriptableObject
    {
        [SerializeField] private MiniGameStats _lowRiskStats;
        [SerializeField] private MiniGameStats _mediumRiskStats;
        [SerializeField] private MiniGameStats _highRiskStats;
        [SerializeField] private float _playTime;
        
        public MiniGameStats LowRiskStats => _lowRiskStats;
        public MiniGameStats MediumRiskStats => _mediumRiskStats;
        public MiniGameStats HighRiskStats => _highRiskStats;
        public float PlayTime => _playTime;

        [Serializable]
        public class MiniGameStats
        {
            [SerializeField, Range(0, 100)] private float _positiveOutcomeChance;
            [SerializeField, Range(0, 100)] private float _negativeOutcomeChance;
            [SerializeField, Range(0, 100)] private float _noneOutcomeChance;
            public float PositiveOutcomeChance => _positiveOutcomeChance;
            public float NegativeOutcomeChance => _negativeOutcomeChance;
            public float NoneOutcomeChance => _noneOutcomeChance;
        }
    }
}
