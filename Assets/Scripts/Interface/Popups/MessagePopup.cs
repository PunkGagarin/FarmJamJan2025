using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Interface.Popups
{
    public class MessagePopup : Popup
    {
        [SerializeField] private TMP_Text _message;
        [SerializeField] private Button _okButton;

        [Inject] private WriterService _writerService;

        public void Open(string message)
        {
            _writerService.WriteText(_message, message);
            Open(true);
        }
        
        private void Awake()
        {
            _okButton.onClick.AddListener(Close);
        }
        
        private void OnDestroy()
        {
            _okButton.onClick.RemoveListener(Close);
        }
    }
}
