using System;
using UnityEngine;

namespace Farm.Gameplay.Definitions
{ 
    [Serializable]
    public class EmbryoChances
    {
        [Range(0, 100)] public float HumanChance;
        [Range(0, 100)] public float AnimalChance;
        [Range(0, 100)] public float FishChance;
    }
}
