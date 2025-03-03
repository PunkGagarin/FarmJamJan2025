using System.Collections.Generic;
using DG.Tweening;
using Farm.Audio;
using Farm.Gameplay.Configs.MiniGame;
using Farm.Gameplay.MiniGame;
using Farm.Interface.Popups;
using Farm.Utils.Timer;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Gameplay
{
    public class MiniGameButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private List<Image> _indicators;
        [SerializeField, Range(0, 1)] private float _minFillAmount;
        [SerializeField, Range(0, 1)] private float _maxFillAmount;
        
        [Inject] private PopupManager _popupManager;
        [Inject] private MiniGameEffectsMediator _miniGameEffectsMediator;
        [Inject] private TimerService _timerService;
        [Inject] private SoundManager _soundManager;

        private List<float> _timeToPlay;
        private List<float> _feelAmount;
        private int _index;
        
        private TimerHandle _effectTime;
        private MiniGameVisual _miniGameVisual;

        private const int PRESAVED_AMOUNTS = 100;
        
        private void Interact()
        {
            if (_effectTime != null) return;
            
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
        
        private void SetupMiniGameEffect(MiniGameEffect miniGameEffect, float effectTime)
        {
            _miniGameEffectsMediator.SetMiniGameEffect(miniGameEffect);
            if (miniGameEffect != null)
            {
                _effectTime = _timerService.AddTimer(effectTime, EffectEnds);
                SetupAnimations();
                _button.interactable = false;
            }
        }
        
        private void SetupAnimations()
        {
            _timeToPlay = new List<float>();
            _feelAmount = new List<float>();
            
            for (int i = 0; i < PRESAVED_AMOUNTS; i++)
            {
                _timeToPlay.Add(Random.Range(0, .5f));
                _feelAmount.Add(Random.Range(_minFillAmount, _maxFillAmount));
            }

            foreach (Image indicator in _indicators)
                PlayAnimation(indicator);
        }

        private void PlayAnimation(Image indicator)
        {
            if (_effectTime == null)
            {
                indicator.DOFillAmount(0, 1f);
                return;
            }
            
            int localIndex = _index % PRESAVED_AMOUNTS;
            indicator.DOFillAmount(_feelAmount[localIndex], _timeToPlay[localIndex])
                .SetEase(Ease.Linear)
                .OnComplete(() => PlayAnimation(indicator));
            _index++;
        }

        private void EffectEnds()
        {
            _miniGameEffectsMediator.SetMiniGameEffect(null);
            _effectTime = null;
            _button.interactable = true;
        }

        public void OnClickTrigger()
        {
            _soundManager.PlaySoundByType(_effectTime == null ? GameAudioType.UiButtonClick : GameAudioType.ActionError, 0);
        }
        
        private void Awake()
        {
            _button.onClick.AddListener(Interact);
        }
        
        private void OnDestroy()
        {
            if (_miniGameVisual != null)
                _miniGameVisual.OnMiniGameEnds -= SetupMiniGameEffect;
            
            _button.onClick.RemoveListener(Interact);
        }
    }
}
