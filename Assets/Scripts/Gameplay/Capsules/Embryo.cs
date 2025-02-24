using Farm.Enums;
using UnityEngine;

namespace Farm.Gameplay.Capsules
{
    public class Embryo
    {
        public EmbryoType EmbryoType { get; set; }
        public int StarvationValue { get; set; }
        public float TimeToGrowth { get; set; }
        public int EnergyValue { get; set; }
        public Sprite Image { get; set; }
        
        public Embryo(EmbryoType embryoType, int starvationValue, float timeToGrowth, int energyValue, Sprite image)
        {
            EmbryoType = embryoType;
            StarvationValue = starvationValue;
            TimeToGrowth = timeToGrowth;
            EnergyValue = energyValue;
            Image = image;
        }
    }
}
