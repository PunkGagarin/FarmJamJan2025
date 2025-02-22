using System;
using System.Collections.Generic;
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
        }
        
        [SerializeField] private string _name;
        [SerializeField] private float _maxSatiety;
        [SerializeField] private float _startSatiety;
        [SerializeField] private float _timeToStarveTick;
        [SerializeField] private float _lifeTime;
        [SerializeField] private List<PhasesData> _satietyPhasesData;
        [SerializeField] private float _rampageTime;
        [SerializeField] private float _humanFeedModifier;
        [SerializeField] private float _animalFeedModifier;
        [SerializeField] private float _fishFeedModifier;

        public string Name => _name;
        public float MaxSatiety => _maxSatiety;
        public float StartSatiety => _startSatiety;
        public float TimeToStarveTick => _timeToStarveTick;
        public List<PhasesData> SatietyPhasesData => _satietyPhasesData;
        public float LifeTime => _lifeTime;
        public float RampageTime => _rampageTime;
        public float HumanFeedModifier => _humanFeedModifier;
        public float AnimalFeedModifier => _animalFeedModifier;
        public float FishFeedModifier => _fishFeedModifier;

    }
}
