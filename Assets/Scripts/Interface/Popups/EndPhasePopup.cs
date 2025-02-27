using System;
using System.Collections;
using System.Text;
using DG.Tweening;
using Farm.Gameplay.Definitions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Interface.Popups
{
    public class EndPhasePopup : Popup
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _header;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _continue;
        [SerializeField] private float _writerCooldown = 0.05f;
        [SerializeField] private float _backgroundShowTime = 1f;
        
        private TheOldOneDefinition _currentGod;
        private string _currentText;
        
        private event Action OnTextWritten;

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
                StartCoroutine(Writer(_header));
                OnTextWritten += WriteGodText;
            });
        }

        private void WriteGodText()
        {
            OnTextWritten -= WriteGodText;
            _currentText = _currentGod.EndPhaseText;
            
            StartCoroutine(Writer(_text));
            OnTextWritten += ShowButton;
        }
        
        private void ShowButton()
        {
            OnTextWritten -= ShowButton;
            _continue.gameObject.SetActive(true);
        }

        private IEnumerator Writer(TMP_Text target)
        {
            target.text = string.Empty;
            target.gameObject.SetActive(true);

            var stringBuilder = new StringBuilder();
            var chars = _currentText.ToCharArray();
            int index = 0;
            do
            {
                stringBuilder.Append(chars[index]);
                target.text = stringBuilder.ToString();
                index++;
                yield return new WaitForSeconds(_writerCooldown);
            }
            while (index < chars.Length);
            OnTextWritten?.Invoke();
        }

        private void HidePopup() => 
            _canvasGroup.DOFade(0, _backgroundShowTime).OnComplete(Close);

        private void Awake() => 
            _continue.onClick.AddListener(HidePopup);

        private void OnDestroy() => 
            _continue.onClick.RemoveListener(HidePopup);
    }
}
