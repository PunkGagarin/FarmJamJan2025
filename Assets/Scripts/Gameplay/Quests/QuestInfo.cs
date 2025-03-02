using System;

namespace Farm.Gameplay.Quests
{
    public class QuestInfo
    {
        public RequirementType RequirementType { get; }
        public int RequiredExtraAmount { get; }
        public int RequiredAmount { get; }
        public int CurrentAmount { get; set; }
        public bool IsCompleted => CurrentAmount >= RequiredAmount;
        public string QuestStateDescription { get; private set; }
        
        private const string PEOPLE_DESCRIPTION = "Collected people:";
        private const string ANIMAL_DESCRIPTION = "Collected animals:";
        private const string FISH_DESCRIPTION = "Collected fish:";
        private const string ENERGY_DESCRIPTION = "Gathered energy:";
        private const string SATIETY_DESCRIPTION = "Gathered satiety:";
        private const string AVAILABLE_CAPSULES_DESCRIPTION = "Unlocked capsules:";
        private const string CAPSULES_WITH_TIER_DESCRIPTION = "Upgraded capsules to [RequiredExtraAmount] tier:";
        private const string MODULES_DESCRIPTION = "Equiped modules to capsules:";
        private const string EMPTY_CAPSULES = "Keep capsules empty for seconds:";
        private const string COLLECT_UNITS_IN_SECONDS = "Collect units in [RequiredExtraAmount] seconds:";
        private const string WIN_IN_THIRD_TIER_MINI_GAME_TIMES = "Win in roulet with high risk times:";
        private const string KEEP_IN_RAGE = "Keep old one in rage for seconds:";

        public QuestInfo(RequirementType requirementType, int requiredAmount, int requiredExtraAmount, string questStateDescription)
        {
            RequirementType = requirementType;
            RequiredAmount = requiredAmount;
            RequiredExtraAmount = requiredExtraAmount;
            CurrentAmount = 0;
            QuestStateDescription = questStateDescription;
        }
    }
}
