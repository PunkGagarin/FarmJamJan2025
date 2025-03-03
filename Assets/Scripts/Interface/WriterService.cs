using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using Zenject;

namespace Farm.Interface
{
    public class WriterService : MonoBehaviour
    {
        private float _writerCooldown = 0.025f;
        private string _textToWrite;
        private TMP_Text _targetContainer;
        private Coroutine _writerCoroutine;
        
        public event Action OnTextWritten;
        
        [Inject]
        private void Construct(float writerCooldown)
        {
            _writerCooldown = writerCooldown;
        }
        
        public void WriteText(TMP_Text target, string text)
        {
            _targetContainer = target;
            _textToWrite = text;
            _writerCoroutine = StartCoroutine(Writer());
        }

        public void StopWriter()
        {
            if (_writerCoroutine != null)
                StopCoroutine(_writerCoroutine);
            
            _targetContainer.text = _textToWrite;
            OnTextWritten?.Invoke();
        }
        
        private IEnumerator Writer()
        {
            var stringBuilder = new StringBuilder();
            var chars = _textToWrite.ToCharArray();
            int index = 0;
            do
            {
                stringBuilder.Append(chars[index]);
                _targetContainer.text = stringBuilder.ToString();
                index++;
                yield return new WaitForSeconds(_writerCooldown);
            }
            while (index < chars.Length);
            StopWriter();
        }
    }
}
