using System.Collections.Generic;
using System.Linq;
using Farm.Gameplay.Quests;
using Farm.Utils.Timer;
using UnityEngine;
using Zenject;

namespace Farm.Gameplay.Capsules
{
    public class CapsuleManager : MonoBehaviour
    {
        [SerializeField] private List<Capsule> _capsules;
        [Inject] private QuestService _questService;
        [Inject] private TimerService _timerService;
        
        private float _emptyCapsulesTime = 0f;
        private int _embryoCollectsLastSeconds = 0;
        private TimerHandle _embryoCollectTimer;
        private QuestInfo _collectUnitsQuest;

        public int CapsulesOwned => _capsules.Count(capsule => capsule.IsOwn);

        public void TutorialOpenFirstCapsule()
        {
            _capsules[0].OpenCapsule();
        }
        
        public void TutorialOpenRemainCapsules()
        {
            for (int i = 1; i < _capsules.Count; i++)
                if (_capsules[i].IsOwn)
                    _capsules[i].OpenCapsule();
        }

        public Capsule NextUnopenedCapsuleCost()
        {
            foreach (Capsule capsule in _capsules)
            {
                if (!capsule.IsOwn)
                    return capsule;
            }
            return null;
        }

        private void Awake()
        {
            Capsule.OnCapsuleBought += CountOwnCapsules;
            Capsule.OnCapsuleUpgrade += CountCapsulesTiers;
            CapsuleSlot.OnAnyModuleChanged += CountCapsuleModules;
            foreach (var capsule in _capsules)
                capsule.OnEmbryoStateChanged += CountEmbryos;
        }
        
        private void CountEmbryos(EmbryoStates newEmbryoState)
        {
            if (newEmbryoState != EmbryoStates.Empty)
                return;

            if (_collectUnitsQuest == null)
            {
                var quests = _questService.GetQuestRequirements();
                
                if (quests == null)
                    return;
            
                foreach (var questInfo in quests)
                {
                    if (questInfo.RequirementType == RequirementType.CollectUnitsInSeconds)
                    {
                        _collectUnitsQuest = questInfo;
                        break;
                    }
                }
            }

            if (_collectUnitsQuest is { IsCompleted: false })
            {
                _embryoCollectsLastSeconds++;
                _questService.SetRequirement(RequirementType.CollectUnitsInSeconds, _embryoCollectsLastSeconds);

                _embryoCollectTimer ??= _timerService.AddTimer(_collectUnitsQuest.RequiredExtraAmount, ResetEmbryoCount, true);
            }
        }
        
        private void ResetEmbryoCount()
        {
            if (_collectUnitsQuest is { IsCompleted: true })
            {
                FinalizeTimer();
                return;
            }
            
            _embryoCollectsLastSeconds = 0;
            _questService.SetRequirement(RequirementType.CollectUnitsInSeconds, _embryoCollectsLastSeconds);
        }
        
        private void FinalizeTimer()
        {
            _embryoCollectTimer?.FinalizeTimer();
            _embryoCollectTimer = null;
        }

        private void CollectQuestInfo()
        {
            CountOwnCapsules();
            CountCapsulesTiers();
            CountCapsuleModules();
        }

        private void CountOwnCapsules()
        {
            _questService.SetRequirement(RequirementType.AvailableCapsules, _capsules.Count(capsule => capsule.IsOwn));
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
                _questService.SetRequirement(RequirementType.CapsulesWithTierUpgrade, keyValuePair.Value, keyValuePair.Key);
        }
    
        private void CountCapsuleModules(CapsuleSlot _) => CountCapsuleModules();
    
        private void CountCapsuleModules()
        {
            int moduleCount = _capsules.Sum(capsule => capsule.CapsuleSlots.Count(slot => slot.UpgradeModule != null));
        
            _questService.SetRequirement(RequirementType.EquippedModules, moduleCount);
        }

        private void OnDestroy()
        {
            Capsule.OnCapsuleBought -= CountOwnCapsules;
            Capsule.OnCapsuleUpgrade -= CountCapsulesTiers;
            CapsuleSlot.OnAnyModuleChanged -= CountCapsuleModules;
            _questService.OnQuestStarted -= CollectQuestInfo;
            foreach (var capsule in _capsules)
                capsule.OnEmbryoStateChanged -= CountEmbryos;
        }

        private void Update()
        {
            CountEmptyCapsulesTime();
        }
        
        private void CountEmptyCapsulesTime()
        {
            foreach (Capsule capsule in _capsules)
            {
                if (!capsule.IsOwn)
                    continue;
                
                if (capsule.Embryo != null)
                {
                    _emptyCapsulesTime = 0f;
                    _questService.SetRequirement(RequirementType.EmptyCapsules, 0);
                    return;
                }
            }

            _emptyCapsulesTime += Time.deltaTime;
            _questService.SetRequirement(RequirementType.EmptyCapsules, Mathf.FloorToInt(_emptyCapsulesTime));
        }
    }
}