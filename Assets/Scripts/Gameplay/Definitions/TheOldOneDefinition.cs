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
            [Tooltip("Количество секунд через сколько начинается этап")] public float PhaseStartTime;
            [Tooltip("Количество сытости которое теряет бог каждый тик голода")] public float SatietyLoseByTick;
            [Tooltip("Квест с которым начинается этап, если квеста нет - этап проходит без квеста")] public QuestDefinition Quest;
        }
        [SerializeField, Tooltip("Иконка бога")] private Sprite _icon;
        [SerializeField, Multiline(2), Tooltip("Информация отображающаяся в заголовке на главном экране")] private string _headerInfo;
        [SerializeField, Tooltip("Максимальное количество сытости")] private float _maxSatiety;
        [SerializeField, Tooltip("Начальное количество сытости")] private float _startSatiety;
        [SerializeField, Tooltip("Периодичность в секундах уменьшения сытости")] private float _timeToStarveTick;
        [SerializeField, Tooltip("Время игры бога в секундах")] private float _lifeTime;
        [SerializeField] private List<PhasesData> _satietyPhasesData;
        [SerializeField, Tooltip("Дополнительное время в секундах до проигрыша после опустошения сытости")] private float _rampageTime;
        [SerializeField, Tooltip("Модификатор даваемой сытости в процентах за людей")] private float _humanSatietyModifier;
        [SerializeField, Tooltip("Модификатор даваемой сытости в процентах за животных")] private float _animalSatietyModifier;
        [SerializeField, Tooltip("Модификатор даваемой сытости в процентах за рыб")] private float _fishSatietyModifier;
        [SerializeField, Tooltip("Текст отображающийся на экране перехода на следующего бога")] private string _endPhaseText;

        public Sprite Icon => _icon;
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
        public string EndPhaseText => _endPhaseText;
    }
}
