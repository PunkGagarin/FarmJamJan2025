using DG.Tweening;
using Farm.Gameplay.Definitions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Interface.Popups
{
    public class EndPhasePopup : Popup
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _header;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _continue;
        [SerializeField] private float _backgroundShowTime = 1f;

        [Inject] private WriterService _writerService;
        
        private TheOldOneDefinition _currentGod;
        private string _currentText;
        
        public void Open(TheOldOneDefinition currentGod)
        {
            Open(true);
            _currentGod = currentGod;
            _currentText = _header.text;
            _header.gameObject.SetActive(false);
            _text.gameObject.SetActive(false);
            _continue.gameObject.SetActive(false);

            _canvasGroup.DOFade(1, 0).OnComplete(() =>
            {
                _header.gameObject.SetActive(true);
                _text.text = string.Empty;      
                _text.gameObject.SetActive(true);
                _writerService.OnTextWritten += WriteGodText;
                _writerService.WriteText(_text, _currentText);
            });
        }

        private void WriteGodText()
        {
            _writerService.OnTextWritten -= WriteGodText;
            _currentText = _currentGod.EndPhaseText;
            
            _text.text = string.Empty;
            _writerService.OnTextWritten += ShowButton;
            _writerService.WriteText(_text, _currentText);
        }
        
        private void ShowButton()
        {
            _writerService.OnTextWritten -= ShowButton;
            _continue.gameObject.SetActive(true);
        }

        private void HidePopup() => 
            _canvasGroup.DOFade(0, _backgroundShowTime).OnComplete(Close);

        private void Awake() => 
            _continue.onClick.AddListener(HidePopup);

        private void OnDestroy() => 
            _continue.onClick.RemoveListener(HidePopup);
    }
}
