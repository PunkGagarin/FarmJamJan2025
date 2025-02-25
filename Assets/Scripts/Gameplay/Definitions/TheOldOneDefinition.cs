using System;
using System.Collections.Generic;
using Farm.Gameplay.Quests;
using UnityEngine;

namespace Farm.Gameplay.Definitions
{
    [CreateAssetMenu(fileName = "The Old One Definition", menuName = "Game Resources/Definition/The Old One")]
    public class TheOldOneDefinition : Definition
    {
        [Serializable]
        public class PhasesData
        {
            public float PhaseStartTime;
            public float SatietyLoseByTick;
            public QuestDefinition Quest;
        }
        
        [SerializeField, Multiline(2), Tooltip("Информация отображающаяся в заголовке")] private string _headerInfo;
        [SerializeField, Tooltip("Максимальное количество сытости")] private float _maxSatiety;
        [SerializeField, Tooltip("Начальное количество сытости")] private float _startSatiety;
        [SerializeField, Tooltip("Периодичность уменьшения сытости")] private float _timeToStarveTick;
        [SerializeField, Tooltip("Время игры бога")] private float _lifeTime;
        [SerializeField] private List<PhasesData> _satietyPhasesData;
        [SerializeField] private float _rampageTime;
        [SerializeField] private float _humanSatietyModifier;
        [SerializeField] private float _animalSatietyModifier;
        [SerializeField] private float _fishSatietyModifier;

        public string HeaderInfo => _headerInfo;
        public float MaxSatiety => _maxSatiety;
        public float StartSatiety => _startSatiety;
        public float TimeToStarveTick => _timeToStarveTick;
        public List<PhasesData> SatietyPhasesData => _satietyPhasesData;
        public float LifeTime => _lifeTime;
        public float RampageTime => _rampageTime;
        public float HumanSatietyModifier => _humanSatietyModifier;
        public float AnimalSatietyModifier => _animalSatietyModifier;
        public float FishSatietyModifier => _fishSatietyModifier;
    }
}
