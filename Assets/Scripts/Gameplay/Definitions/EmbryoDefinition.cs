using Farm.Enums;
using UnityEngine;

namespace Farm.Gameplay.Definitions
{
    [CreateAssetMenu(fileName = "Embryo Definition", menuName = "Game Resources/Definition/Embryo")]
    public class EmbryoDefinition : Definition
    {
        [SerializeField] private Sprite _image;
        [SerializeField] private float _timeToGrowth;
        [SerializeField] private int _energyValue;
        [SerializeField] private int _starvationValue;
        [SerializeField] private EmbryoType _embryoType;
        
        public Sprite Image => _image;
        public float TimeToGrowth => _timeToGrowth;
        public int EnergyValue => _energyValue;
        public int StarvationValue => _starvationValue;
        public EmbryoType EmbryoType => _embryoType;
    }
}
