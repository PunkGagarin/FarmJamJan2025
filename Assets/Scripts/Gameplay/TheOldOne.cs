using System;
using DG.Tweening;
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
        private TheOldOneDefinition _definition;
        private float _maxSatiety;
        private float _currentSatiety;
        private TimerHandle _lifeTimer;
        private TimerHandle _starveTimer;
        private TimerHandle _rampageTimer;
        private int _currentPhase;
        private Sequence _blinkingTween;
        private bool _inRampage;

        public event SatietyChangeHandler OnSatietyChanged;
        public event Action<int> OnPhaseChanged;

        public void Initialize(TheOldOneDefinition definition)
        {
            _definition = definition;
            _inRampage = false;

            _currentPhase = 0;
            _currentSatiety = _definition.StartSatiety;
            _maxSatiety = _definition.MaxSatiety;

            _lifeTimer = _timerService.AddTimer(_definition.LifeTime, Sealed);
            _timerService.AddTimer(_definition.SatietyPhasesData[_currentPhase + 1].PhaseStartTime, ChangePhase);
            OnPhaseChanged?.Invoke(_currentPhase);
            _starveTimer = _timerService.AddTimer(_definition.TimeToStarveTick, Starve, true);
            OnSatietyChanged?.Invoke(_currentSatiety, _maxSatiety);

            InitializeInterface();
        }

        public void Feed(int amount)
        {
            _currentSatiety += amount;
            if (_currentSatiety > _definition.MaxSatiety) 
                _currentSatiety = _definition.MaxSatiety;
            
            OnSatietyChanged?.Invoke(_currentSatiety, _maxSatiety);
        }
        
        private void Sealed()
        {
            _lifeTimer?.FinalizeTimer();
            _starveTimer?.FinalizeTimer();
            _rampageTimer?.FinalizeTimer();
            
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
            int nextPhase = _currentPhase + 1;
            if (nextPhase >= _definition.SatietyPhasesData.Count)
                return;

            float timerTime = _definition.SatietyPhasesData[nextPhase].PhaseStartTime - _definition.SatietyPhasesData[_currentPhase].PhaseStartTime;
            _currentPhase = nextPhase;
            Debug.Log($"phase changed to {_currentPhase}");
            _timerService.AddTimer(timerTime, ChangePhase);
            OnPhaseChanged?.Invoke(_currentPhase);
        }

        private void Starve()
        {
            _currentSatiety -= _definition.SatietyPhasesData[_currentPhase].SatietyLoseByTick;
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
