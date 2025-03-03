using System;
using Audio;
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
    public class TheOldOne : MonoBehaviour
    {
        public delegate void SatietyChangeHandler(float current, float max);

        [SerializeField] private SpriteRenderer _icon;
        [SerializeField] private TheOldOneUI _theOldOneUI;

        [Inject] private PopupManager _popupManager;
        [Inject] private TimerService _timerService;
        [Inject] private FeedMediator _feedMediator;
        [Inject] private QuestService _questService;
        [Inject] private SoundManager _sfxManager;
        [Inject] private MusicManager _musicManager;

        private TheOldOneDefinition _definition;
        private float _maxSatiety;
        private float _currentSatiety;
        private TimerHandle _lifeTimer;
        private TimerHandle _starveTimer;
        private TimerHandle _rampageTimer;
        private TimerHandle _phaseTimer;
        private int _currentStage;
        private bool _inRampage;
        private bool _waitForTutorial = true;

        public event SatietyChangeHandler OnSatietyChanged;
        public event Action<int> OnPhaseChanged;
        public event Action OnSealed;
        public event Action OnDefeat;
        public event Action<bool> OnRampageStateChanged; 

        public void Initialize(TheOldOneDefinition definition)
        {
            _musicManager.PlayBgm();
            _definition = definition;
            _inRampage = false;
            _icon.sprite = definition.Icon;

            SetupStats();

            SetupTimers();

            OnPhaseChanged += UpdateQuest;
            UpdateQuest(_currentStage);

            InitializeInterface();
        }

        public void TutorialCompleted()
        {
            _lifeTimer.SpeedMultiplier = 1f;
            _phaseTimer.SpeedMultiplier = 1f;
            _starveTimer.SpeedMultiplier = 1f;
            _waitForTutorial = false;
            _theOldOneUI.TutorialComplete();
        }

        private void UpdateQuest(int stage)
        {
            _questService.FailQuest();
            _questService.SetupQuest(
                _definition.SatietyPhasesData[stage].Quest == null 
                    ? null 
                    : _definition.SatietyPhasesData[stage].Quest);
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
            
            if (_waitForTutorial)
            {
                _lifeTimer.SpeedMultiplier = 0;
                _phaseTimer.SpeedMultiplier = 0;
                _starveTimer.SpeedMultiplier = 0;
            }
        }

        public void Feed(int amount, EmbryoType embryoType)
        {
            float modifier = 0;

            switch (embryoType)
            {
                case EmbryoType.Human:
                    _questService.AddRequirement(RequirementType.Human);
                    modifier = _definition.HumanSatietyModifier;
                    break;
                case EmbryoType.Animal:
                    _questService.AddRequirement(RequirementType.Animal);
                    modifier = _definition.AnimalSatietyModifier;
                    break;
                case EmbryoType.Fish:
                    _questService.AddRequirement(RequirementType.Fish);
                    modifier = _definition.FishSatietyModifier;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(embryoType), embryoType, null);
            }

            AddSatiety(Mathf.RoundToInt(amount * modifier / 100f));

            _questService.SetRequirement(RequirementType.Satiety, (int)_currentSatiety);
        }

        [ContextMenu("seal")]
        private void Sealed()
        {
            if (_inRampage)
                StopRampage();
            
            _starveTimer?.FinalizeTimer();
            _lifeTimer?.EarlyComplete(true);
            _phaseTimer?.EarlyComplete(true);

            _lifeTimer = null;
            _starveTimer = null;
            _phaseTimer = null;
            
            if (_questService.HaveActiveQuest)
                _questService.ExpireQuest();

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
                float phaseTime = _definition.SatietyPhasesData[_currentStage + 1].PhaseStartTime -
                                  _definition.SatietyPhasesData[_currentStage].PhaseStartTime;
                _phaseTimer = _timerService.AddTimer(phaseTime, ChangePhase);
            }
        }

        private void Starve() => 
            RemoveSatiety((int)_definition.SatietyPhasesData[_currentStage].SatietyLoseByTick);

        private void Rampage()
        {
            OnRampageStateChanged?.Invoke(true);
           
            _inRampage = true;

            _musicManager.SetNextClipToPlay(GameAudioType.RampageBgm);

            _rampageTimer ??= _timerService.AddTimer(_definition.RampageTime, Defeat);
        }

        private void StopRampage()
        {
            _inRampage = false;
            OnRampageStateChanged?.Invoke(false);
            _rampageTimer.EarlyComplete(true);
            _rampageTimer = null;
            _musicManager.SetNextClipToPlay(GameAudioType.GamePlayBgm);
        }

        private void Defeat()
        {
            _inRampage = false;

            _sfxManager.PlaySoundByType(GameAudioType.GameOver, 0);
            OnDefeat?.Invoke();
        }

        private void CollectQuestInfo()
        {
            _questService.SetRequirement(RequirementType.Satiety, (int)_currentSatiety);
        }

        private void SatietyChanged(float current, float _)
        {
            if (current <= 0)
            {
                Rampage();
            }

            if (current > 0 && _inRampage)
                StopRampage();
        }

        private void AddSatiety(int amount)
        {
            _currentSatiety += amount;
            if (_currentSatiety >= _maxSatiety)
            {
                _currentSatiety = _maxSatiety;
                Sealed();
            }
            
            OnSatietyChanged?.Invoke(_currentSatiety, _maxSatiety);
        }
        
        private void RemoveSatiety(int amount)
        {
            _currentSatiety -= amount;
            if (_currentSatiety < 0)
                _currentSatiety = 0;
            
            OnSatietyChanged?.Invoke(_currentSatiety, _maxSatiety);
        }
        
        private void Awake()
        {
            _feedMediator.SetupTheOldOne(this);
            _questService.OnQuestStarted += CollectQuestInfo;
            _questService.OnQuestFailed += RemoveSatiety;
            OnSatietyChanged += SatietyChanged;
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
            _questService.OnQuestStarted -= CollectQuestInfo;
            _questService.OnQuestFailed -= RemoveSatiety;
            OnSatietyChanged -= SatietyChanged;
        }

        private void Update()
        {
            _questService.SetRequirement(RequirementType.KeepRampageMode, _inRampage ? Mathf.FloorToInt(_rampageTimer.Duration - _rampageTimer.RemainingTime) : 0);
        }
    }
}
