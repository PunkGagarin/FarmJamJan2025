using Farm.Gameplay.Configs.MiniGame;
using Farm.Gameplay.MiniGame;
using Farm.Interface.Popups;
using Farm.Utils.Timer;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Farm.Gameplay
{
    public class EffectsMachine : MonoBehaviour
    {
        [Inject] private PopupManager _popupManager;
        [Inject] private MiniGameEffectsMediator _miniGameEffectsMediator;
        [Inject] private TimerService _timerService;

        private TimerHandle _effectTime;
        private MiniGameVisual _miniGameVisual;

        private void OnMouseDown()
        {
            if (IsPointerOverUI())
                return;

            Interact();
        }
        
        private void Interact()
        {
            if (_effectTime != null)
                return;
            
            if (_miniGameVisual == null)
            {
                _miniGameVisual = _popupManager.OpenMiniGame().MiniGameVisual;
                _miniGameVisual.OnMiniGameEnds += SetupMiniGameEffect;
            }
            else
            {
                _popupManager.OpenMiniGame();
            }
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
        
        private void SetupMiniGameEffect(MiniGameEffect miniGameEffect, float effectTime)
        {
            _miniGameEffectsMediator.SetMiniGameEffect(miniGameEffect);
            if (miniGameEffect != null)
                _effectTime = _timerService.AddTimer(effectTime, EffectEnds);
        }
        
        private void EffectEnds()
        {
            _miniGameEffectsMediator.SetMiniGameEffect(null);
            _effectTime = null;
        }

        private void OnDestroy()
        {
            _miniGameVisual.OnMiniGameEnds -= SetupMiniGameEffect;
        }
    }
}
