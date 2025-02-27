using System;
using System.Collections.Generic;
using DG.Tweening;
using Farm.Enums;
using Farm.Gameplay.Definitions;
using Farm.Gameplay.Quests;
using Farm.Interface.Popups;
using Farm.Interface.TheOldOne;
using Farm.Utils.Pause;
using Farm.Utils.Timer;
using UnityEngine;
using Zenject;

namespace Farm.Gameplay
{
    public class TheOldOne : MonoBehaviour, IPauseHandler
    {
        public delegate void SatietyChangeHandler(float current, float max);
        [SerializeField] private SpriteRenderer _icon;
        [SerializeField] private TheOldOneUI _theOldOneUI;

        [Inject] private PopupManager _popupManager;
        [Inject] private TimerService _timerService;
        [Inject] private FeedMediator _feedMediator;
        [Inject] private QuestProvider _questProvider;
        private TheOldOneDefinition _definition;
        private float _maxSatiety;
        private float _currentSatiety;
        private TimerHandle _lifeTimer;
        private TimerHandle _starveTimer;
        private TimerHandle _rampageTimer;
        private TimerHandle _phaseTimer;
        private int _currentStage;
        private Sequence _blinkingTween;
        private bool _inRampage;

        public event SatietyChangeHandler OnSatietyChanged;
        public event Action<int> OnPhaseChanged;
        public event Action OnSealed;
        public event Action OnDefeat;

        public void Initialize(TheOldOneDefinition definition)
        {
            _definition = definition;
            _inRampage = false;
            _icon.sprite = definition.Icon;

            SetupStats();

            SetupTimers();

            OnPhaseChanged += UpdateQuest;
            UpdateQuest(_currentStage);

            InitializeInterface();
        }

        private void UpdateQuest(int stage)
        {
            _questProvider.FinalizeQuest();
            if (_definition.SatietyPhasesData[stage].Quest == null)
            {
                _questProvider.SetupQuest(null);
                _theOldOneUI.UpdateQuestButtonAction(null);
            }
            else
            {
                _questProvider.SetupQuest(_definition.SatietyPhasesData[stage].Quest);
                _theOldOneUI.UpdateQuestButtonAction(OpenQuestPopup);
            }
        }

        private void OpenQuestPopup()
        {
            (string, List<QuestInfo>) questPopupInfo = _questProvider.GetQuestDescriptionAndInfos();
            _popupManager.OpenQuest(questPopupInfo.Item1, questPopupInfo.Item2);
        }

        private void SetupStats()
        {
            _currentStage = 0;
            _currentSatiety = _definition.StartSatiety;
            _maxSatiety = _definition.MaxSatiety;
            OnSatietyChanged?.Invoke(_currentSatiety, _maxSatiety);
        }

        private void SetupTimers()
        {
            _lifeTimer = _timerService.AddTimer(_definition.LifeTime, Sealed);
            _phaseTimer = _timerService.AddTimer(_definition.SatietyPhasesData[_currentStage + 1].PhaseStartTime, ChangePhase);
            _starveTimer = _timerService.AddTimer(_definition.TimeToStarveTick, Starve, true);
        }

        public void Feed(int amount, EmbryoType embryoType)
        {
            float modifier = 0;

            switch (embryoType)
            {
                case EmbryoType.Human:
                    _questProvider.AddRequirement(RequirementType.Human);
                    modifier = _definition.HumanSatietyModifier;
                    break;
                case EmbryoType.Animal:
                    _questProvider.AddRequirement(RequirementType.Animal);
                    modifier = _definition.AnimalSatietyModifier;
                    break;
                case EmbryoType.Fish:
                    _questProvider.AddRequirement(RequirementType.Fish);
                    modifier = _definition.FishSatietyModifier;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(embryoType), embryoType, null);
            }

            _currentSatiety += amount * modifier / 100f;
            if (_currentSatiety > _definition.MaxSatiety)
                _currentSatiety = _definition.MaxSatiety;

            _questProvider.SetRequirement(RequirementType.Satiety, (int)_currentSatiety);
            OnSatietyChanged?.Invoke(_currentSatiety, _maxSatiety);
        }

        private void Sealed()
        {
            _lifeTimer?.FinalizeTimer();
            _starveTimer?.FinalizeTimer();
            _rampageTimer?.FinalizeTimer();
            _phaseTimer?.FinalizeTimer();

            _lifeTimer = null;
            _starveTimer = null;
            _rampageTimer = null;
            _phaseTimer = null;
            
            OnSealed?.Invoke();
        }

        private void InitializeInterface()
        {
            _theOldOneUI.Initialize(_definition, _lifeTimer);
            OnPhaseChanged += _theOldOneUI.PhaseChanged;
            OnSatietyChanged += _theOldOneUI.UpdateSatietyBar;
        }

        private void ChangePhase()
        {
            _currentStage++;
            Debug.Log($"Stage changed to {_currentStage}");
            OnPhaseChanged?.Invoke(_currentStage);

            if (_currentStage + 1 < _definition.SatietyPhasesData.Count)
            {
                float phaseTime = _definition.SatietyPhasesData[_currentStage + 1].PhaseStartTime - _definition.SatietyPhasesData[_currentStage].PhaseStartTime;
                _phaseTimer = _timerService.AddTimer(phaseTime, ChangePhase);
            }
        }

        private void Starve()
        {
            _currentSatiety -= _definition.SatietyPhasesData[_currentStage].SatietyLoseByTick;
            OnSatietyChanged?.Invoke(_currentSatiety, _maxSatiety);
        }

        private void Rampage()
        {
            _blinkingTween = DOTween.Sequence();

            _blinkingTween.Append(_icon.DOFade(0, 1));
            _blinkingTween.Append(_icon.DOFade(1, 1)).OnComplete(() => _blinkingTween.Restart());

            _blinkingTween.Restart();
            _inRampage = true;
            
            if (_rampageTimer == null)
                _rampageTimer = _timerService.AddTimer(_definition.RampageTime, Defeat);
            else 
                _rampageTimer.SetManualPause(false);
        }
        
        private void StopRampage()
        {
            _inRampage = false;
            _rampageTimer.SetManualPause(true);
            _icon.DOFade(1, 0);
            _blinkingTween.Kill();
        }

        private void Defeat()
        {
            _inRampage = false;
            _blinkingTween.Kill();
            
            OnDefeat?.Invoke();
        }

        public void SetPaused(bool isPaused)
        {
            if (_inRampage)
                _blinkingTween.TogglePause();
        }
        
        private void CollectQuestInfo()
        {
            _questProvider.SetRequirement(RequirementType.Satiety, (int)_currentSatiety);
        }
        
        private void SatietyChanged(float current, float _)
        {
            if (current <= 0)
                Rampage();

            if (current > 0 && _inRampage)
                StopRampage();
        }

        private void Awake()
        {
            _feedMediator.SetupTheOldOne(this);
            _questProvider.OnQuestStarted += CollectQuestInfo;
            _questProvider.OnQuestFailed += OnQuestFailed;
            OnSatietyChanged += SatietyChanged;
        }
        
        private void OnQuestFailed(int satietyPenalty)
        {
            _currentSatiety -= satietyPenalty;
            OnSatietyChanged?.Invoke(_currentSatiety, _maxSatiety);
        }

        private void OnDestroy()
        {
            _starveTimer?.FinalizeTimer();
            _rampageTimer?.FinalizeTimer();
            _lifeTimer?.FinalizeTimer();
            _starveTimer = null;
            _rampageTimer = null;
            _lifeTimer = null;
            OnPhaseChanged -= _theOldOneUI.PhaseChanged;
            OnSatietyChanged -= _theOldOneUI.UpdateSatietyBar;
            _blinkingTween.Kill();
            _questProvider.OnQuestStarted -= CollectQuestInfo;
            OnSatietyChanged -= SatietyChanged;
        }
    }
}
