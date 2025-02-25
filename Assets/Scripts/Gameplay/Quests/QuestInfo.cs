using System;

namespace Farm.Gameplay.Quests
{
    public class QuestInfo
    {
        public RequirementType RequirementType { get; }
        public int RequiredTier { get; }
        public int RequiredAmount { get; }
        public int CurrentAmount { get; set; }
        public bool IsCompleted => CurrentAmount >= RequiredAmount;
        
        private const string PEOPLE_DESCRIPTION = "Collected people:";
        private const string ANIMAL_DESCRIPTION = "Collected animals:";
        private const string FISH_DESCRIPTION = "Collected fish:";
        private const string ENERGY_DESCRIPTION = "Gathered energy:";
        private const string SATIETY_DESCRIPTION = "Gathered satiety:";
        private const string AVAILABLE_CAPSULES_DESCRIPTION = "Unlocked capsules:";
        private const string CAPSULES_WITH_TIER_DESCRIPTION = "Upgraded capsules to [RequiredTier] tier:";
        private const string MODULES_DESCRIPTION = "Equiped modules to capsules:";

        public QuestInfo(RequirementType requirementType, int requiredAmount, int requiredTier)
        {
            RequirementType = requirementType;
            RequiredAmount = requiredAmount;
            RequiredTier = requiredTier;
            CurrentAmount = 0;
        }

        public string GetCurrentQuestDescription()
        {
            return RequirementType switch
            {
                RequirementType.Human => PEOPLE_DESCRIPTION,
                RequirementType.Animal => ANIMAL_DESCRIPTION,
                RequirementType.Fish => FISH_DESCRIPTION,
                RequirementType.Energy => ENERGY_DESCRIPTION,
                RequirementType.Satiety => SATIETY_DESCRIPTION,
                RequirementType.AvailableCapsules => AVAILABLE_CAPSULES_DESCRIPTION,
                RequirementType.CapsulesWithTierUpgrade => CAPSULES_WITH_TIER_DESCRIPTION,
                RequirementType.EquippedModules => MODULES_DESCRIPTION,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
