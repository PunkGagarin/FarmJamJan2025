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
        [SerializeField] private Segment _segmentPrefab;
        [SerializeField] private Transform _segmentsContainer;
        
        [Inject] private TimerService _timerService;
        public event Action<MiniGameEffect> OnMiniGameEnds;
        private bool _isStarted;
        private TimerHandle _miniGameTimer;
        private TimerHandle _delayTimer;
        private Queue<Segment> _segments = new();
        private float _totalAngle = 0f;
        private int _currentTapCount = 0;
        
        private const string START_GAME = "Start Game";
        private const string TAP = "Tap me!";
        private const string END_GAME = "Collect!";
        private const float FULL_CIRCLE_ANGLE = 360f;
        private const int MAX_ATTEMPTS_TO_FIT_SEGMENT = 10;
        

        public void Initialize()
        {
            ResetState();

            _actionButtonText.text = START_GAME;
            _actionButton.onClick.RemoveAllListeners();
            _actionButton.onClick.AddListener(StartGame);
            _miniGameSpeedometer.Init(_miniGameConfig);
        }
        
        private void ResetState()
        {
            _miniGameTimer?.Reset();
            _delayTimer?.Reset();
            foreach (Segment segment in _segments)
                segment.gameObject.SetActive(false);
            _totalAngle = 0f;
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
            _delayTimer?.EarlyComplete();
            DisableActionButton();
            _actionButtonText.text = END_GAME;
            _actionButton.onClick.RemoveListener(TapAction);
            _actionButton.onClick.AddListener(EndGame);
        }
        
        private void EndGame()
        {
            _isStarted = false;
            bool isCollected = false;
            var drumShift = _drum.transform.rotation.eulerAngles.z;
            _currentTapCount = 0;
            foreach (Segment segment in _segments)
            {
                if ((segment.StartAngle - drumShift) % FULL_CIRCLE_ANGLE <= 0 && 
                    (segment.StartAngle + segment.Angle - drumShift) % FULL_CIRCLE_ANGLE >= 0)
                {
                    isCollected = true;
                    OnMiniGameEnds?.Invoke(segment.MiniGameEffect);
                    break;
                }
            }
            if (!isCollected)
                OnMiniGameEnds?.Invoke(null);
        }

        private void TapAction()
        {
            DisableActionButton();
            _currentTapCount += 1;
            
            _speed = Mathf.Min(_speed + _miniGameConfig.SpeedToAddPerTap, _miniGameConfig.MaxSpeed);
            _miniGameSpeedometer.SetSpeed(_speed);
            GenerateSegment();
            if (_currentTapCount >= _miniGameConfig.MaxTaps)
            {
                FinalizeGame();
            }
        }
        
        private void GenerateSegment()
        {
            int allowedTiers = _miniGameSpeedometer.AllowTiers;
            List<MiniGameEffect> allowedEffects = _miniGameConfig.Effects.Where(buffs => buffs.Tier <= allowedTiers).ToList();
            
            bool segmentAdded = false;
            int attempts = 0;

            while (!segmentAdded && attempts < MAX_ATTEMPTS_TO_FIT_SEGMENT) 
            {
                MiniGameEffect randomEffect = allowedEffects[Random.Range(0, allowedEffects.Count)];
                float startAngle = Random.Range(0f, FULL_CIRCLE_ANGLE);
                float segmentSize = DeterminateFillFromBuff(randomEffect);
                
                if (!IsSegmentOverlap(startAngle, segmentSize))
                {
                    Segment segment = PickSegment();
                    
                    segment.SetStartAngle(startAngle);
                    segment.SetSegmentAngle(segmentSize);
                    segment.SetEffect(randomEffect);
                    _totalAngle += segmentSize; 
                    segmentAdded = true;
                }
                else
                {
                    float fitSegmentAngle = allowedTiers switch
                    {
                        1 => _miniGameConfig.Tier1Angle,
                        2 => _miniGameConfig.Tier2Angle,
                        3 => _miniGameConfig.Tier3Angle,
                        _ => 0
                    };
                    
                    if (!CanFitSegment(fitSegmentAngle))
                        ResetFirstSegment();
                }
                
                attempts++;
            }
        }
        
        private bool CanFitSegment(float segmentSize)
        {
            List<float> gaps = new List<float>();

            if (_segments.Count == 0)
                return true;

            var sortedSegments = _segments.OrderBy(segment => segment.StartAngle).ToList();

            for (int i = 0; i < sortedSegments.Count; i++)
            {
                float currentSegmentEndAngle = sortedSegments[i].StartAngle + sortedSegments[i].Angle;
                float nextSegmentStartAngle = (i + 1) < sortedSegments.Count ? sortedSegments[i + 1].StartAngle : 360f;

                float gap = i + 1 < sortedSegments.Count ? nextSegmentStartAngle - currentSegmentEndAngle : (360f + sortedSegments[0].StartAngle) - currentSegmentEndAngle;
                gaps.Add(gap);
            }

            return gaps.Any(gap => gap >= segmentSize);
        }
        
        private Segment PickSegment()
        {
            if (_segments.Count == _miniGameConfig.MaxSegmentsOnWheel)
                return ResetFirstSegment();
            
            foreach (Segment segment in _segments)
            {
                if (segment.Angle == 0)
                    return segment;
            }

            Segment newSegment = Instantiate(_segmentPrefab, _drum.position, Quaternion.identity, _segmentsContainer);
            _segments.Enqueue(newSegment);
            return newSegment;
        }
        
        private Segment ResetFirstSegment()
        {
            var segmentToReturn = _segments.Dequeue();
            segmentToReturn.gameObject.SetActive(true);
            _segments.Enqueue(segmentToReturn);
            _totalAngle -= segmentToReturn.Angle;
            segmentToReturn.SetSegmentAngle(0);
            return segmentToReturn;
        }

        private bool IsSegmentOverlap(float startAngle, float segmentSize)
        {
            foreach (var segment in _segments)
            {
                float segmentEndAngle = segment.StartAngle + segment.Angle;
        
                if ((startAngle >= segment.StartAngle && startAngle < segmentEndAngle) ||
                    (segment.StartAngle >= startAngle && segment.StartAngle < startAngle + segmentSize))
                    return true;
            }
            return false;
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
