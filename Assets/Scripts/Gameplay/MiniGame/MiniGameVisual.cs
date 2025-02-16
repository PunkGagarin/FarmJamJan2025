using System;
using Farm.Utils.Timer;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Farm.Gameplay.MiniGame
{
    public class MiniGameVisual : MonoBehaviour
    {
        [SerializeField] private MockMiniGameInfo _miniGameInfo;
        [SerializeField] private Button _lowRisk;
        [SerializeField] private Button _mediumRisk;
        [SerializeField] private Button _highRisk;
        [SerializeField] private GameObject _visualRoot;
        [SerializeField] private Image _timer;
        
        [Inject] private TimerService _timerService;
        
        private TimerHandle _timerHandle;

        public event Action<string> OnMiniGameEnds;

        private void Start()
        {
            _lowRisk.onClick.AddListener(StartMiniGameLowRisk);
            _mediumRisk.onClick.AddListener(StartMiniGameMediumRisk);
            _highRisk.onClick.AddListener(StartMiniGameHighRisk);
        }

        private void Update()
        {
            if (_timerHandle != null)
                _timer.fillAmount = 1 - _timerHandle.Progress;
        }

        private void OnDestroy()
        {
            _lowRisk.onClick.RemoveListener(StartMiniGameLowRisk);
            _mediumRisk.onClick.RemoveListener(StartMiniGameMediumRisk);
            _highRisk.onClick.RemoveListener(StartMiniGameHighRisk);
        }
        
        private void StartMiniGameLowRisk()
        {
            _visualRoot.SetActive(true);
            _timerHandle = _timerService.AddTimer(_miniGameInfo.PlayTime, LowRiskMiniGameEnds);
        }
        
        private void StartMiniGameMediumRisk()
        {
            _visualRoot.SetActive(true);
            _timerHandle = _timerService.AddTimer(_miniGameInfo.PlayTime, MediumRiskMiniGameEnds);
        }
        
        private void StartMiniGameHighRisk()
        {
            _visualRoot.SetActive(true);
            _timerHandle = _timerService.AddTimer(_miniGameInfo.PlayTime, HighRiskMiniGameEnds);
        }

        private void LowRiskMiniGameEnds()
        {
            _visualRoot.SetActive(false);
            OnMiniGameEnds?.Invoke(CalculateRisk(_miniGameInfo.LowRiskStats.NegativeOutcomeChance, _miniGameInfo.LowRiskStats.PositiveOutcomeChance, _miniGameInfo.LowRiskStats.NoneOutcomeChance));
            _timerHandle = null;
        }
        
        private void MediumRiskMiniGameEnds()
        {
            _visualRoot.SetActive(false);
            OnMiniGameEnds?.Invoke(CalculateRisk(_miniGameInfo.MediumRiskStats.NegativeOutcomeChance, _miniGameInfo.MediumRiskStats.PositiveOutcomeChance, _miniGameInfo.MediumRiskStats.NoneOutcomeChance));
            _timerHandle = null;
        }
        
        private void HighRiskMiniGameEnds()
        {
            _visualRoot.SetActive(false);
            OnMiniGameEnds?.Invoke(CalculateRisk(_miniGameInfo.HighRiskStats.NegativeOutcomeChance, _miniGameInfo.HighRiskStats.PositiveOutcomeChance, _miniGameInfo.HighRiskStats.NoneOutcomeChance));
            _timerHandle = null;
        }

        private string CalculateRisk(float negativeChance, float positiveChance, float neutralChance)
        {
            float totalChance = negativeChance + positiveChance + neutralChance;

            float proc = Random.Range(0, totalChance);
            if (proc <= negativeChance)
                return "negative";
            
            if (proc <= negativeChance + positiveChance)
                return "positive";

            return "neutral";
        }

    }
}
