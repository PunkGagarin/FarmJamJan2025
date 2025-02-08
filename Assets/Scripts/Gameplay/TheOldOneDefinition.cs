using UnityEngine;

namespace Farm.Gameplay
{
    [CreateAssetMenu(fileName = "The Old One Definition", menuName = "Game Resources/The Old One Definition")]
    public class TheOldOneDefinition : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private float _startSatiety;
        [SerializeField] private float _timeToStarve;
        [SerializeField] private float _satietyLoseByTick;
        
        public string Name => _name;
        public float StartSatiety => _startSatiety;
        public float TimeToStarve => _timeToStarve;
        public float SatietyLoseByTick => _satietyLoseByTick;
    }
}
