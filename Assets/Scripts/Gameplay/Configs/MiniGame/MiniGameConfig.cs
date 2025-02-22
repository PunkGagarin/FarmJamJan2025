using System.Collections.Generic;
using UnityEngine;

namespace Farm.Gameplay.Configs.MiniGame
{
    [CreateAssetMenu(fileName = "Minigame Config", menuName = "Game Resources/Configs/Minigame")]
    public class MiniGameConfig : ScriptableObject
    {
        [SerializeField] private MiniGamePool _pool;
        [SerializeField, Range(5, 60)] private float _tier1Angle;
        [SerializeField, Range(5, 60)] private float _tier2Angle;
        [SerializeField, Range(5, 60)] private float _tier3Angle;
        [SerializeField] private float _startSpeed;
        [SerializeField] private float _speedToAddPerTap;
        [SerializeField] private float _maxSpeed;
        [SerializeField] private int _maxSegmentsOnWheel;
        [SerializeField] private float _playTime;
        [SerializeField] private float _delayTime;
        [SerializeField, Range(0, 1), Tooltip("Процент заполнения шкалы скорости до получение следующего тира мутаций")] private float _tier1SpeedCap, _tier2SpeedCap;

        public List<MiniGameEffect> Effects => _pool.Effects;
        public float Tier1Angle => _tier1Angle;
        public float Tier2Angle => _tier2Angle;
        public float Tier3Angle => _tier3Angle;
        public float StartSpeed => _startSpeed;
        public float SpeedToAddPerTap => _speedToAddPerTap;
        public float MaxSpeed => _maxSpeed;
        public int MaxSegmentsOnWheel => _maxSegmentsOnWheel;
        public float PlayTime => _playTime;
        public float DelayTime => _delayTime;
        public float Tier1SpeedCap => _tier1SpeedCap;
        public float Tier2SpeedCap => _tier2SpeedCap;
    }
}
