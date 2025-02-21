using System;
using System.Collections.Generic;
using System.Linq;
using Farm.Gameplay.Configs.MiniGame;
using Farm.Utils.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Farm.Gameplay.MiniGame
{
    public class MiniGameVisual : MonoBehaviour
    {
        [SerializeField] private MiniGameConfig _miniGameConfig;
        [SerializeField] private Transform _drum;
        [SerializeField] private float _speed;
        [SerializeField] private Button _actionButton;
        [SerializeField] private TMP_Text _actionButtonText;
        [SerializeField] private MiniGameSpeedometer _miniGameSpeedometer;
        [SerializeField] private Color _positiveColor;
        [SerializeField] private Color _negativeColor;
        [SerializeField] private Segment _segmentPrefab;
        [SerializeField] private Transform _segmentsContainer;
        [Inject] private TimerService _timerService;
        public event Action<MiniGameEffect> OnMiniGameEnds;
        private bool _isStarted;
        private TimerHandle _miniGameTimer;
        private TimerHandle _delayTimer;
        private List<Segment> _segments = new List<Segment>();
        
        private const string START_GAME = "Start Game";
        private const string TAP = "Tap me!";
        private const string END_GAME = "Collect!";
        

        public void Initialize()
        {
            _actionButtonText.text = START_GAME;
            _actionButton.onClick.RemoveAllListeners();
            _actionButton.onClick.AddListener(StartGame);
            _miniGameSpeedometer.Init(_miniGameConfig);
        }
        
        private void StartGame()
        {
            _isStarted = true;
            _actionButtonText.text = TAP;
            _actionButton.onClick.RemoveListener(StartGame);
            _actionButton.onClick.AddListener(TapAction);
            SetupGame();
        }
        
        private void SetupGame()
        {
            _miniGameTimer = _timerService.AddTimer(_miniGameConfig.PlayTime, FinalizeGame);
            _speed = _miniGameConfig.StartSpeed;
        }
        
        private void FinalizeGame()
        {
            _delayTimer.EarlyComplete();
            DisableActionButton();
            _actionButtonText.text = END_GAME;
            _actionButton.onClick.RemoveListener(TapAction);
            _actionButton.onClick.AddListener(EndGame);
        }
        
        private void EndGame()
        {
            _isStarted = false;
        }

        private void TapAction()
        {
            DisableActionButton();
            _speed = Mathf.Min(_speed + _miniGameConfig.SpeedToAddPerTap, _miniGameConfig.MaxSpeed);
            _miniGameSpeedometer.SetSpeed(_speed);
            GenerateSegment();
        }
        
        private void GenerateSegment()
        {
            int allowedTiers = _miniGameSpeedometer.AllowTiers;
            List<MiniGameEffect> allowedEffects = _miniGameConfig.Effects.Where(buffs => buffs.Tier <= allowedTiers).ToList();
            MiniGameEffect randomEffect = allowedEffects[Random.Range(0, allowedEffects.Count)];
            Segment segment = Instantiate(_segmentPrefab, _drum.position, Quaternion.identity, _segmentsContainer);
            float segmentFill = DeterminateFillFromBuff(randomEffect)  / 360f;
            segment.SetSegmentFill(segmentFill);
            segment.GetComponent<Image>().color = randomEffect.Value > 0 ? _positiveColor : _negativeColor;
        }
        private float DeterminateFillFromBuff(MiniGameEffect randomEffect)
        {
            switch (randomEffect.Tier)
            {
                case 1:
                    return _miniGameConfig.Tier1Angle;
                case 2:
                    return _miniGameConfig.Tier2Angle;
                case 3:
                    return _miniGameConfig.Tier3Angle;
                default:
                    return 0;
            }
            
        }

        private void DisableActionButton()
        {
            _delayTimer = _timerService.AddTimer(_miniGameConfig.DelayTime, () => _actionButton.interactable = true);
            _actionButton.interactable = false;
        }


        private void Update()
        {
            if (!_isStarted) 
                return;
            
            _drum.Rotate(0, 0, _speed * Time.deltaTime);
        }
    }
}
