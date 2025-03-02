using System;
using System.Collections.Generic;
using System.Linq;

namespace Farm.Gameplay.Quests
{
    public class QuestService
    {
        private QuestDefinition _currentQuest;
        private List<QuestInfo> _currentQuestRequirements = new();
        private bool _isQuestCompleted;
        
        public event Action OnQuestStarted;
        public event Action<int> OnQuestFailed;
        public event Action<int> OnQuestCompleted;
        public event Action OnQuestUpdated;

        public void SetupQuest(QuestDefinition questDefinition)
        {
            _currentQuest = questDefinition;
            _isQuestCompleted = false;
            if (questDefinition == null)
            {
                _currentQuestRequirements = null;
                return;
            }
            
            _currentQuestRequirements = new List<QuestInfo>();
            foreach (QuestRequirement questRequirement in questDefinition.Requirements)
            {
                QuestInfo questInfo = new QuestInfo(questRequirement.RequirementType, questRequirement.RequiredAmount, questRequirement.RequiredExtraAmount, questRequirement.QuestStateDescription);
                _currentQuestRequirements.Add(questInfo);
            }
            
            OnQuestStarted?.Invoke();
            OnQuestUpdated?.Invoke();
        }

        public void AddRequirement(RequirementType requirementType, int requiredTier = -1)
        {
            if (_currentQuestRequirements == null)
                return;
            
            foreach (QuestInfo questInfo in _currentQuestRequirements)
            {
                if (questInfo.RequirementType != requirementType)
                    continue;
                
                if (requirementType == RequirementType.CapsulesWithTierUpgrade && requiredTier != questInfo.RequiredExtraAmount)
                    continue;
                
                if (questInfo.IsCompleted)
                    continue;
                
                questInfo.CurrentAmount++;
            }
            OnQuestUpdated?.Invoke();
        }

        public void SetRequirement(RequirementType requirementType, int value, int requiredTier = -1)
        {
            if (_currentQuestRequirements == null || _isQuestCompleted)
                return;
            
            foreach (QuestInfo questInfo in _currentQuestRequirements)
            {
                if (questInfo.RequirementType != requirementType)
                    continue;
                
                if (requirementType == RequirementType.CapsulesWithTierUpgrade && requiredTier != questInfo.RequiredExtraAmount)
                    continue;
                
                if (questInfo.IsCompleted)
                    continue;

                questInfo.CurrentAmount = value;
            }
            
            if (_currentQuestRequirements.All(questRequirement => questRequirement.IsCompleted))
            {
                _isQuestCompleted = true;
                OnQuestCompleted?.Invoke(_currentQuest.EnergyReward);
            }
            OnQuestUpdated?.Invoke();
        }
        
        public List<QuestInfo> GetQuestRequirements() => 
            _currentQuestRequirements;
        
        public void FinalizeQuest()
        {
            if (_currentQuest == null || _currentQuestRequirements == null)
                return;
            
            OnQuestFailed?.Invoke(_currentQuest.SatietyPenalty);
        }
    }
}
