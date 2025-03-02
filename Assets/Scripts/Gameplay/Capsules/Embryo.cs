using Farm.Enums;

namespace Farm.Gameplay.Capsules
{
    public class Embryo
    {
        public EmbryoType EmbryoType { get; set; }
        public int StarvationValue { get; set; }
        public float TimeToGrowth { get; set; }
        public int EnergyValue { get; set; }
        
        public Embryo(EmbryoType embryoType, int starvationValue, float timeToGrowth, int energyValue)
        {
            EmbryoType = embryoType;
            StarvationValue = starvationValue;
            TimeToGrowth = timeToGrowth;
            EnergyValue = energyValue;
        }
    }
}
