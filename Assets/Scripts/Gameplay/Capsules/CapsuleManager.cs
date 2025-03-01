using System.Collections.Generic;
using System.Linq;
using Farm.Gameplay.Quests;
using UnityEngine;
using Zenject;

namespace Farm.Gameplay.Capsules
{
    public class CapsuleManager : MonoBehaviour
    {
        [SerializeField] private List<Capsule> _capsules;
        [Inject] private QuestProvider _questProvider;

        private void Awake()
        {
            Capsule.OnCapsuleBought += CountOwnCapsules;
            Capsule.OnCapsuleUpgrade += CountCapsulesTiers;
            CapsuleSlot.OnAnyModuleChanged += CountCapsuleModules;
        }

        private void CollectQuestInfo()
        {
            CountOwnCapsules();
            CountCapsulesTiers();
            CountCapsuleModules();
        }

        private void CountOwnCapsules()
        {
            _questProvider.SetRequirement(RequirementType.AvailableCapsules, _capsules.Count(capsule => capsule.IsOwn));
        }

        private void CountCapsulesTiers()
        {
            Dictionary<int, int> capsuleTiers = new Dictionary<int, int>();
            foreach (Capsule capsule in _capsules)
            {
                if (!capsule.IsOwn)
                    continue;
            
                if (!capsuleTiers.TryAdd(capsule.Tier, 1))
                    capsuleTiers[capsule.Tier] += 1;
            }

            foreach (KeyValuePair<int,int> keyValuePair in capsuleTiers)
                _questProvider.SetRequirement(RequirementType.CapsulesWithTierUpgrade, keyValuePair.Value, keyValuePair.Key);
        }
    
        private void CountCapsuleModules(CapsuleSlot _) => CountCapsuleModules();
    
        private void CountCapsuleModules()
        {
            int moduleCount = _capsules.Sum(capsule => capsule.CapsuleSlots.Count(slot => slot.UpgradeModule != null));
        
            _questProvider.SetRequirement(RequirementType.EquippedModules, moduleCount);
        }

        private void OnDestroy()
        {
            Capsule.OnCapsuleBought -= CountOwnCapsules;
            Capsule.OnCapsuleUpgrade -= CountCapsulesTiers;
            CapsuleSlot.OnAnyModuleChanged -= CountCapsuleModules;
            _questProvider.OnQuestStarted -= CollectQuestInfo;
        }
    }
}