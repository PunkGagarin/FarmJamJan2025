using System;
using DG.Tweening;
using Farm.Enums;
using Farm.Gameplay.Definitions;
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
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private TheOldOneUI _theOldOneUI;

        [Inject] private PopupManager _popupManager;
        [Inject] private TimerService _timerService;
        [Inject] private FeedMediator _feedMediator;
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

        public void Initialize(TheOldOneDefinition definition)
        {
            _definition = definition;
            _inRampage = false;

            _currentStage = 0;
            _currentSatiety = _definition.StartSatiety;
            _maxSatiety = _definition.MaxSatiety;

            _lifeTimer = _timerService.AddTimer(_definition.LifeTime, Sealed);
            _phaseTimer = _timerService.AddTimer(_definition.SatietyPhasesData[_currentStage + 1].PhaseStartTime, ChangePhase);
            _starveTimer = _timerService.AddTimer(_definition.TimeToStarveTick, Starve, true);
            OnSatietyChanged?.Invoke(_currentSatiety, _maxSatiety);

            _feedMediator.UpdateTheOldOne(this);

            InitializeInterface();
        }

        public void Feed(int amount, EmbryoType embryoType)
        {
            float modifier = 0;

            switch (embryoType)
            {
                case EmbryoType.Human:
                    modifier = _definition.HumanSatietyModifier;
                    break;
                case EmbryoType.Animal:
                    modifier = _definition.AnimalSatietyModifier;
                    break;
                case EmbryoType.Fish:
                    modifier = _definition.FishSatietyModifier;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(embryoType), embryoType, null);
            }
            
            _currentSatiety += amount * modifier;
            if (_currentSatiety > _definition.MaxSatiety)
                _currentSatiety = _definition.MaxSatiety;

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

            Debug.Log($"The old one sealed!");
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

            if (_currentSatiety <= 0)
            {
                _starveTimer?.FinalizeTimer();

                Rampage();
            }
        }

        private void Rampage()
        {
            _blinkingTween = DOTween.Sequence();

            _blinkingTween.Append(_sprite.DOFade(0, 1));
            _blinkingTween.Append(_sprite.DOFade(1, 1)).OnComplete(() => _blinkingTween.Restart());

            _blinkingTween.Restart();
            _inRampage = true;
            _rampageTimer = _timerService.AddTimer(_definition.RampageTime, Defeat);
        }

        private void Defeat()
        {
            _inRampage = false;
            _blinkingTween.Kill();

            _popupManager.OpenGameOver();
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
        }

        public void SetPaused(bool isPaused)
        {
            if (_inRampage)
                _blinkingTween.TogglePause();
        }
    }
}
