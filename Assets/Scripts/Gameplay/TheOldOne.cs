using Farm.Interface.Popups;
using Farm.Utils.Timer;
using UnityEngine;
using Zenject;

namespace Farm.Gameplay
{
    public class TheOldOne : MonoBehaviour
    {
        public delegate void SatietyChangeHandler(float current, float max);
        
        [Inject] private PopupManager _popupManager;
        [Inject] private TimerService _timerService;
        private TheOldOneDefinition _definition;
        private float _currentSatiety;
        private TimerHandle _starveTimer;

        public event SatietyChangeHandler OnStarveProcess; 

        public void Initialize(TheOldOneDefinition definition)
        {
            _definition = definition;
            _currentSatiety = _definition.StartSatiety;
            _starveTimer = _timerService.AddTimer(_definition.TimeToStarve, Starve, true);
            OnStarveProcess?.Invoke(_currentSatiety, _definition.StartSatiety);
        }
        
        private void Starve()
        {
            _currentSatiety -= _definition.SatietyLoseByTick;
            OnStarveProcess?.Invoke(_currentSatiety, _definition.StartSatiety);
            
            if (_currentSatiety <= 0)
            {
                _starveTimer?.FinalizeTimer();
                _popupManager.OpenGameOver();
            }
        }

        private void OnDestroy()
        {
            _starveTimer?.FinalizeTimer();
            OnStarveProcess = null;
        }
    }
}
