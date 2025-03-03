using System;
using System.Collections.Generic;
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
        [SerializeField] private List<MessagePopupExtraSettings> _extraSettings;
        [SerializeField] private RectTransform _cutoff;
        [SerializeField] private RectTransform _extraCutoff;
        [SerializeField] private RectTransform _window;

        [Inject] private WriterService _writerService;

        public void Open(string message, int positions = -1)
        {
            if (positions >= 0 && positions < _extraSettings.Count)
            {
                _cutoff.anchoredPosition = _extraSettings[positions].CutoffPosition;
                _cutoff.sizeDelta = _extraSettings[positions].CutoffSize;
                _window.anchoredPosition = _extraSettings[positions].WindowPosition;
                _extraCutoff.anchoredPosition = _extraSettings[positions].ExtraCutoffPosition;
                _extraCutoff.sizeDelta = _extraSettings[positions].ExtraCutoffSize;
            }
            else
            {
                _cutoff.anchoredPosition = Vector2.zero;
                _cutoff.sizeDelta = Vector2.zero;
                _window.anchoredPosition = Vector2.zero;
                _extraCutoff.anchoredPosition = Vector2.zero;
                _extraCutoff.sizeDelta = Vector2.zero;
            }
            _writerService.WriteText(_message, message);
            Open(true);
        }

        protected override void Close()
        {
            _writerService.StopWriter();
            base.Close();
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

    [Serializable]
    public class MessagePopupExtraSettings
    {
        [SerializeField] private Vector3 _windowPosition;
        [SerializeField] private Vector3 _cutoffPosition;
        [SerializeField] private Vector2 _cutoffSize;
        [SerializeField] private Vector3 _extraCutoffPosition;
        [SerializeField] private Vector2 _extraCutoffSize;
        public Vector3 WindowPosition => _windowPosition;
        public Vector3 CutoffPosition => _cutoffPosition;
        public Vector2 CutoffSize => _cutoffSize;
        public Vector3 ExtraCutoffPosition => _extraCutoffPosition;
        public Vector2 ExtraCutoffSize => _extraCutoffSize;
    }
}
