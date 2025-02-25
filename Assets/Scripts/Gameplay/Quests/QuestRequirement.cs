using System;
using UnityEngine;

namespace Farm.Gameplay.Quests
{
    [Serializable]
    public class QuestRequirement
    {
        public RequirementType RequirementType;
        [Min(0)] public int RequiredAmount;
        [Range(0, 3)] public int RequiredTier;
    }
}
