using Farm.Utils.Pause;
using Farm.Utils.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Utils
{
    public class TimerTest : MonoBehaviour
    {
        [SerializeField] private Image _countdown;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private float _duration;
        [SerializeField] private float _timeToAdd;
        [SerializeField] private float _multiplierToAdd;
        [SerializeField] private Color _startColor;
        [SerializeField] private Color _endColor;
        [Inject] private TimerService _timer;
        [Inject] private PauseService _pauseService;
        private TimerHandle _timerHandle;
        private bool _isPause = false;

        private void Update()
        {
            if (_timerHandle is { RemainingTime: > 0 })
            {
                float progress = _timerHandle.Progress;
                _countdown.fillAmount = progress;
                _countdown.color = Color.Lerp(_endColor, _startColor, progress);

                _timeText.text = _timerHandle.RemainingTime.ToString("0.0");
                _timeText.color = Color.Lerp(_endColor, _startColor, progress);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log($"Запустили таймер");
                StartTimer();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log($"Добавляем время к таймеру");
                _timerHandle.AddTime(_timeToAdd);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log($"Отнимаем время у таймера");
                _timerHandle.RemoveTime(_timeToAdd);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log(_isPause ? $"Снимаю с паузы" : "Ставлю на паузу");
                _isPause = !_isPause;
                _pauseService.SetPaused(_isPause);
            }
            

            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log($"Ускоряем таймер на {_multiplierToAdd}");
                _timerHandle.SpeedMultiplier += _multiplierToAdd;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log($"Возвращаем нормальное течение времени");
                _timerHandle.SpeedMultiplier = 1;
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log($"Замедляем таймер на {_multiplierToAdd}");
                _timerHandle.SpeedMultiplier -= _multiplierToAdd;
            }
        }

        private void StartTimer()
        {
            if (_timerHandle != null)
                _timer.RemoveTimer(_timerHandle);
            
            _timerHandle = _timer.AddTimer(_duration, TimerExpire);
        }

        private void TimerExpire()
        {
            _countdown.fillAmount = 0;
            _timeText.text = "пупупу";
            _timeText.color = _endColor;
            _countdown.color = _endColor;
            Debug.Log($"Отчет закончен");
        }
    }
}
