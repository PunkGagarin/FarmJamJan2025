using Farm.Gameplay.Repositories;
using Farm.Interface.Popups;
using Farm.Interface.TheOldOne;
using UnityEngine;
using Zenject;

namespace Farm.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TheOldOne _theOldOne;
        [SerializeField] private TheOldOneUI _theOldOneUI;
        [SerializeField] private bool _withTutorial;
        
        [Inject] private TheOldOneRepository _theOldOneRepository;
        [Inject] private PopupManager _popupManager;
        
        private int _currentStage = 0;
        
        public void SetupOldOne() => 
            _theOldOne.Initialize(_theOldOneRepository.Definitions[_currentStage]);
        
        private void Awake()
        {
            _currentStage = 0;
            _theOldOne.OnSealed += TheOldOneSealed;
            _theOldOne.OnDefeat += Defeat;
            _theOldOne.OnRampageStateChanged += RampageStateChanged;
        }

        private void Start()
        {
            if (!_withTutorial)
                SetupOldOne();
        }

        private void RampageStateChanged(bool newState)
        {
            _theOldOneUI.OnRampageStateChanged(newState);
        }

        private void TheOldOneSealed()
        {
            _currentStage++;
            if (_currentStage >= _theOldOneRepository.Definitions.Count)
            {
                WinGame();
            }
            else
            {
                _popupManager.OpenEndPhasePopup(_theOldOneRepository.Definitions[_currentStage - 1]);
                SetupOldOne();
            }
            
        }

        private void Defeat()
        {
            _popupManager.OpenGameOver();
        }

        private void WinGame()
        {
            _popupManager.OpenVictoryPopup();
        }

        private void OnDestroy()
        {
            _theOldOne.OnSealed -= TheOldOneSealed;
            _theOldOne.OnDefeat -= Defeat;
        }
    }
}
