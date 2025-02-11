using Farm.Utils.Pause;
using UnityEngine;
using Zenject;

namespace Farm.Interface.Popups
{
    public abstract class Popup : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [Inject] private PauseService _pauseService;
        private bool _openedWithPause;

        public RectTransform RectTransform => _rectTransform;

        public virtual void Open(bool withPause)
        {
            _openedWithPause = withPause;
            
            if (withPause)
                _pauseService.SetPaused(true);
            
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            if (_openedWithPause)
                _pauseService.SetPaused(false);

            gameObject.SetActive(false);
        }
    }
}
