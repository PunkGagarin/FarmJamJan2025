using System.Collections.Generic;
using System.Linq;
using Audio;
using Farm.Gameplay.Configs.MiniGame;
using Farm.Interface;
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
        public delegate void MiniGameEnds(MiniGameEffect miniGameEffect, float effectTime);
        
        [SerializeField] private Button _lowRiskButton;
        [SerializeField] private Button _mediumRiskButton;
        [SerializeField] private Button _highRiskButton;
        [SerializeField] private TMP_Text _lowRiskCost;
        [SerializeField] private TMP_Text _mediumRiskCost;
        [SerializeField] private TMP_Text _highRiskCost;
        [SerializeField] private MiniGameConfig _miniGameConfig;
        [SerializeField] private Transform _drum;
        [SerializeField] private Button _actionButton;
        [SerializeField] private TMP_Text _actionButtonText;
        [SerializeField] private MiniGameSpeedometer _miniGameSpeedometer;
        [SerializeField] private Segment _segmentPrefab;
        [SerializeField] private Transform _segmentsContainer;
        
        [Inject] private TimerService _timerService;
        [Inject] private InventoryUI _inventory;
        [Inject] private SoundManager _soundManager;
        
        public event MiniGameEnds OnMiniGameEnds;
        private bool _isStarted;
        private bool _isPaused;
        private bool _isEnded;
        private Queue<Segment> _segments = new();
        private float _speed;
        private float _deceleration;
        private MiniGameRisk _selectedRisk;
        
        private const float FULL_CIRCLE_ANGLE = 360f;

        public void Initialize()
        {
            ResetState();
            
            InitializeButtons();

            _miniGameSpeedometer.Init(_miniGameConfig);
        }
        
        private void InitializeButtons()
        {
            _actionButton.onClick.RemoveAllListeners();
            _actionButton.gameObject.SetActive(false);
            _lowRiskButton.gameObject.SetActive(true);
            _mediumRiskButton.gameObject.SetActive(true);
            _highRiskButton.gameObject.SetActive(true);
            
            _lowRiskButton.onClick.AddListener(StartLowRiskGame);
            _mediumRiskButton.onClick.AddListener(StartMediumRiskGame);
            _highRiskButton.onClick.AddListener(StartHighRiskGame);
            
            _lowRiskCost.text = _miniGameConfig.LowRiskStats.CostToRun.ToString();
            _mediumRiskCost.text = _miniGameConfig.MediumRiskStats.CostToRun.ToString();
            _highRiskCost.text = _miniGameConfig.HighRiskStats.CostToRun.ToString();
        }

        private void ResetState()
        {
            _isEnded = false;
            foreach (Segment segment in _segments)
                segment.gameObject.SetActive(false);
        }
        
        private void StartLowRiskGame()
        {
            if (_inventory.CanBuy(_miniGameConfig.LowRiskStats.CostToRun))
            {
                _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
                _inventory.CurrentEnergy -= _miniGameConfig.LowRiskStats.CostToRun;
                _selectedRisk = _miniGameConfig.LowRiskStats;
                StartGame();
            }
            else
            {
                _soundManager.PlaySoundByType(GameAudioType.ActionError, 0);
                _inventory.ShakeCanNotBuy();
            }
        }
        
        private void StartMediumRiskGame()
        {
            if (_inventory.CanBuy(_miniGameConfig.MediumRiskStats.CostToRun))
            {
                _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
                _inventory.CurrentEnergy -= _miniGameConfig.MediumRiskStats.CostToRun;
                _selectedRisk = _miniGameConfig.MediumRiskStats;
                StartGame();
            }
            else
            {
                _soundManager.PlaySoundByType(GameAudioType.ActionError, 0);
                _inventory.ShakeCanNotBuy();
            }
        }
        
        private void StartHighRiskGame()
        {
            if (_inventory.CanBuy(_miniGameConfig.HighRiskStats.CostToRun))
            {
                _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
                _inventory.CurrentEnergy -= _miniGameConfig.HighRiskStats.CostToRun;
                _selectedRisk = _miniGameConfig.HighRiskStats;
                StartGame();
            }
            else
            {
                _soundManager.PlaySoundByType(GameAudioType.ActionError, 0);
                _inventory.ShakeCanNotBuy();
            }
        }

        private void StartGame()
        {
            GenerateSegment();
            _speed = _selectedRisk.Speed;
            _isStarted = true;
            _actionButton.gameObject.SetActive(true);
            _lowRiskButton.gameObject.SetActive(false);
            _mediumRiskButton.gameObject.SetActive(false);
            _highRiskButton.gameObject.SetActive(false);
            _actionButton.onClick.AddListener(StartDeceleration);
        }
        
        private void StartDeceleration()
        {
            _isStarted = false;
            _isEnded = true;
            _deceleration = _speed / _miniGameConfig.DecelerationTime;
            _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
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
            
            _timerService.AddTimer(1f, () => OnMiniGameEnds?.Invoke(selectedEffect, _miniGameConfig.EffectTime));
        }
        
        private void GenerateSegment()
        {
            List<MiniGameEffect> allowedEffects = _miniGameConfig.Effects.Where(buffs => buffs.Tier <= _selectedRisk.HighestAllowedTier).ToList();

            for (int i = 0; i < _selectedRisk.SegmentsOnWheel; i++)
            {
                MiniGameEffect randomEffect = GetRandomEffect(allowedEffects);
                //Debug.Log($"Random effect: {randomEffect.BuffType}, {randomEffect.Value}");
                float startAngle = FULL_CIRCLE_ANGLE / _selectedRisk.SegmentsOnWheel * i;
                float segmentSize = DeterminateFillFromBuff(randomEffect);
                
                Segment segment = PickSegment();
                    
                segment.SetStartAngle(startAngle);
                segment.SetSegmentAngle(segmentSize);
                segment.SetEffect(randomEffect);
            }
        }
        
        private MiniGameEffect GetRandomEffect(List<MiniGameEffect> allowedEffects)
        {
            float totalWeight = allowedEffects.Sum(effect => effect.Weight);
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;
            //Debug.Log($"total weight = {totalWeight}, random value = {randomValue}");
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
            if (_segments.Count == _selectedRisk.SegmentsOnWheel)
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
        
        private float DeterminateFillFromBuff(MiniGameEffect randomEffect) =>
            randomEffect.Tier switch
            {
                1 => _miniGameConfig.Tier1Angle,
                2 => _miniGameConfig.Tier2Angle,
                3 => _miniGameConfig.Tier3Angle,
                _ => 0
            };

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
