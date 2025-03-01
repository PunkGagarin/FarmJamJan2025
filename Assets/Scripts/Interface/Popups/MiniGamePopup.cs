using Audio;
using Farm.Gameplay.Configs.MiniGame;
using Farm.Gameplay.MiniGame;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Interface.Popups
{
    public class MiniGamePopup : Popup
    {
        [SerializeField] private MiniGameVisual _miniGameVisual;
        [SerializeField] private Button _closeButton;
        [Inject] private SoundManager _soundManager;
        public MiniGameVisual MiniGameVisual => _miniGameVisual;

        public void Open()
        {
            _miniGameVisual.Initialize();
            
            base.Open(false);
        }

        private void Awake()
        {
            _miniGameVisual.OnMiniGameEnds += Close;
            _closeButton.onClick.AddListener(CloseClick);
        }

        private void CloseClick()
        {
            _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
            Close();
        }

        private void Close(MiniGameEffect _, float effectTime) => 
            Close();

        private void OnDestroy()
        {
            _miniGameVisual.OnMiniGameEnds -= Close;
            _closeButton.onClick.RemoveListener(CloseClick);
        }
    }
}