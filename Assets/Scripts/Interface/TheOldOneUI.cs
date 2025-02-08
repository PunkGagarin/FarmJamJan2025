using DG.Tweening;
using Farm.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Interface
{
    public class TheOldOneUI : MonoBehaviour
    {
        [SerializeField] private Image _timer;
        [SerializeField] private float _timeToSpin;
        [SerializeField] private float _endTimerPosition = 360f;
        [SerializeField] private float _startTimerPosition = 45f;

        public void Bind(TheOldOne theOldOne)
        {
            theOldOne.OnStarveProcess += UpdateValues;
        }
        
        private void UpdateValues(float current, float max)
        {
            float progress = current / max;
            Vector3 endValue = new Vector3(0, 0, -(_endTimerPosition - _startTimerPosition) * progress);
            _timer.transform.DORotate(endValue, _timeToSpin);
        }
    }
}
