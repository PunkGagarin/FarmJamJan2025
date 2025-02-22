using System;
using UnityEngine;

namespace Farm.Gameplay.Configs.MiniGame
{
    [Serializable]
    public class MiniGameEffect
    {
        [Range(1, 3)] public int Tier;
        public BuffType BuffType;
        public int Value;
    }
}