using System;
using Farm.Utils.Pause;
using UnityEngine;
using Zenject;

namespace Farm.Interface.Popups
{
    public abstract class Popup : MonoBehaviour
    {
        [Inject] private PauseService _pauseService;
        private bool _openedWithPause;
        
        private event Action OnClose;

        public void SetCloseEvent(Action onClose) => 
            OnClose = onClose;
        
        public virtual void Open(bool withPause)
        {
            _openedWithPause = withPause;
            OnClose = null;
            
            if (withPause)
                _pauseService.SetPaused(true);
            
            gameObject.SetActive(true);
        }

        protected virtual void Close()
        {
            if (_openedWithPause)
                _pauseService.SetPaused(false);

            gameObject.SetActive(false);
            OnClose?.Invoke();
        }

    }
}
