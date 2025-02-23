using System;
using System.Collections.Generic;
using System.Linq;
using Farm.Gameplay.Configs.MiniGame;
using Farm.Utils.Pause;
using Farm.Utils.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Farm.Gameplay.MiniGame
{
    public class MiniGameVisual : MonoBehaviour, IPauseHandler
    {
        [SerializeField] private MiniGameConfig _miniGameConfig;
        [SerializeField] private Transform _drum;
        [SerializeField] private Button _actionButton;
        [SerializeField] private TMP_Text _actionButtonText;
        [SerializeField] private MiniGameSpeedometer _miniGameSpeedometer;
        [SerializeField] private Segment _segmentPrefab;
        [SerializeField] private Transform _segmentsContainer;
        
        [Inject] private TimerService _timerService;
        
        public event Action<MiniGameEffect> OnMiniGameEnds;
        private bool _isStarted;
        private bool _isPaused;
        private bool _isEnded;
        private TimerHandle _miniGameTimer;
        private TimerHandle _delayTimer;
        private Queue<Segment> _segments = new();
        private int _currentTapCount = 0;
        private int _currentTier, _maxTier;
        private float _speed;
        private float _deceleration;
        
        private const string START_GAME = "Start Game";
        private const string TAP = "Tap me!";
        private const string END_GAME = "Collect!";
        private const float FULL_CIRCLE_ANGLE = 360f;
        private const int MAX_ATTEMPTS_TO_FIT_SEGMENT = 10;

        public void Initialize(int currentTier, int maxTier)
        {
            _currentTier = currentTier;
            _maxTier = maxTier;
            ResetState();
            
            _actionButtonText.text = START_GAME;
            _actionButton.onClick.RemoveAllListeners();
            _actionButton.onClick.AddListener(StartGame);
            _miniGameSpeedometer.Init(_miniGameConfig);
        }
        
        private void ResetState()
        {
            _currentTapCount = 0;
            _isEnded = false;
            _miniGameTimer?.EarlyComplete(true);
            _delayTimer?.EarlyComplete();
            _miniGameTimer = null;
            _delayTimer = null;
            foreach (Segment segment in _segments)
                segment.gameObject.SetActive(false);
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
            _miniGameTimer?.EarlyComplete(true);
            _delayTimer?.EarlyComplete();
            _miniGameTimer = null;
            _delayTimer = null;
            DisableActionButton();
            _actionButtonText.text = END_GAME;
            _actionButton.onClick.RemoveListener(TapAction);
            _actionButton.onClick.AddListener(StartDeceleration);
        }
        
        private void StartDeceleration()
        {
            _isStarted = false;
            _isEnded = true;
            _deceleration = _speed / _miniGameConfig.DecelerationTime;
        }
        
        private void DeterminateMiniGameOutput()
        {
            _isEnded = false;
            var drumShift = _drum.transform.rotation.eulerAngles.z;
            MiniGameEffect selectedEffect = null;
            foreach (Segment segment in _segments)
            {
                if ((segment.StartAngle - drumShift) % FULL_CIRCLE_ANGLE <= 0 && 
                    (segment.StartAngle + segment.Angle - drumShift) % FULL_CIRCLE_ANGLE >= 0)
                {
                    selectedEffect = segment.MiniGameEffect;
                    break;
                }
            }
            
            _timerService.AddTimer(1f, () => OnMiniGameEnds?.Invoke(selectedEffect));
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
            List<MiniGameEffect> updateTierEffects = allowedEffects.Where(effect => effect.BuffType == BuffType.UpdateTier).ToList();
            updateTierEffects.ForEach(updateTier =>
            {
                if (updateTier.Value + _currentTier > _maxTier || updateTier.Value + _currentTier < 0)
                    allowedEffects.Remove(updateTier);
            });
            
            bool segmentAdded = false;
            int attempts = 0;

            while (!segmentAdded && attempts < MAX_ATTEMPTS_TO_FIT_SEGMENT)
            {
                MiniGameEffect randomEffect = GetRandomEffect(allowedEffects);
                float startAngle = Random.Range(0f, FULL_CIRCLE_ANGLE);
                float segmentSize = DeterminateFillFromBuff(randomEffect);
                
                if (!IsSegmentOverlap(startAngle, segmentSize))
                {
                    Segment segment = PickSegment();
                    
                    segment.SetStartAngle(startAngle);
                    segment.SetSegmentAngle(segmentSize);
                    segment.SetEffect(randomEffect);
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
        
        private MiniGameEffect GetRandomEffect(List<MiniGameEffect> allowedEffects)
        {
            float totalWeight = allowedEffects.Sum(effect => effect.Weight);
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;

            foreach (MiniGameEffect miniGameEffect in allowedEffects)
            {
                currentWeight += miniGameEffect.Weight;
                
                if (randomValue < currentWeight)
                    return miniGameEffect;
            }
            
            return allowedEffects[^1];
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
            _delayTimer = _timerService.AddTimer(_miniGameConfig.DelayTimeBetweenTaps, () => _actionButton.interactable = true);
            _actionButton.interactable = false;
        }

        private void Update()
        {
            if (_isPaused)
                return;

            if (_isEnded)
            {
                _speed -= _deceleration * Time.deltaTime;
                _drum.Rotate(0, 0, _speed * Time.deltaTime);
                
                if (_speed <= 0)
                    DeterminateMiniGameOutput();
            }
            
            if (_isStarted) 
                _drum.Rotate(0, 0, _speed * Time.deltaTime);
        }
        
        public void SetPaused(bool isPaused) => 
            _isPaused = isPaused;
    }
}
