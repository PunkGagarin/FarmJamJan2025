﻿using Farm.Gameplay.Repositories;
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
        
        [Inject] private TheOldOneRepository _theOldOneRepository;
        [Inject] private PopupManager _popupManager;
        
        private int _currentStage = 0;
        
        private void Start()
        {
            SetupOldOne();
            _theOldOne.OnSealed += TheOldOneSealed;
            _theOldOne.OnDefeat += Defeat;
            _theOldOne.OnRampageStateChanged += RampageStateChanged;
        }
        
        private void RampageStateChanged(bool newState)
        {
            _theOldOneUI.OnRampageStateChanged(newState);
            
        }

        private void TheOldOneSealed()
        {
            _popupManager.OpenEndPhasePopup(_theOldOneRepository.Definitions[_currentStage]);

            _currentStage++;
            
            if (_currentStage >= _theOldOneRepository.Definitions.Count)
            {
                WinGame();
            }
            else
            {
                SetupOldOne();
            }
        }

        private void Defeat()
        {
            _popupManager.OpenGameOver();
        }
        
        private void SetupOldOne() => 
            _theOldOne.Initialize(_theOldOneRepository.Definitions[_currentStage]);

        private void WinGame()
        {
            Debug.Log($"Победа");
            //TODO: Показывать окно победы
        }


        private void OnDestroy()
        {
            _theOldOne.OnSealed -= TheOldOneSealed;
            _theOldOne.OnDefeat -= Defeat;
        }
    }
}
