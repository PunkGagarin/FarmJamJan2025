using System;
using System.Collections.Generic;
using System.Linq;
using Farm.Audio;
using Farm.Gameplay.Configs.MiniGame;
using Farm.Gameplay.Quests;
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
        [SerializeField] private TMP_Text _output;
        
        [Inject] private TimerService _timerService;
        [Inject] private InventoryUI _inventory;
        [Inject] private SoundManager _soundManager;
        [Inject] private QuestService _questService;

        private bool _isStarted;
        private bool _isPaused;
        private bool _isEnded;
        private Queue<Segment> _segments = new();
        private float _speed;
        private float _deceleration;
        private MiniGameRisk _selectedRisk;
        private bool _isFirstRun = true;
        private MiniGameTierType _currentTierType;
        private int _timesWonInARow;
        private float _drumShift;

        private const float FULL_CIRCLE_ANGLE = 360f;
        
        public static event Action OnTutorialMiniGameOpened;
        public event MiniGameEnds OnMiniGameEnds;

        public void Initialize()
        {
            _output.text = "";

            ResetState();

            InitializeButtons();

            _miniGameSpeedometer.Init(_miniGameConfig);
            OnTutorialMiniGameOpened?.Invoke();
        }
        
        private void InitializeButtons()
        {
            _actionButton.gameObject.SetActive(false);
            _lowRiskButton.gameObject.SetActive(true);
            _mediumRiskButton.gameObject.SetActive(true);
            _highRiskButton.gameObject.SetActive(true);
            
            
            _lowRiskCost.text = _miniGameConfig.LowRiskStats.CostToRun.ToString();
            _mediumRiskCost.text = _miniGameConfig.MediumRiskStats.CostToRun.ToString();
            _highRiskCost.text = _miniGameConfig.HighRiskStats.CostToRun.ToString();
        }
        
        private void Awake()
        {
            _lowRiskButton.onClick.AddListener(StartLowRiskGame);
            _mediumRiskButton.onClick.AddListener(StartMediumRiskGame);
            _highRiskButton.onClick.AddListener(StartHighRiskGame);
            _actionButton.onClick.AddListener(StartDeceleration);
        }

        private void OnDestroy()
        {
            _lowRiskButton.onClick.RemoveListener(StartLowRiskGame);
            _mediumRiskButton.onClick.RemoveListener(StartMediumRiskGame);
            _highRiskButton.onClick.RemoveListener(StartHighRiskGame);
            _actionButton.onClick.RemoveListener(StartDeceleration);
        }

        private void ResetState()
        {
            _isEnded = false;
            foreach (Segment segment in _segments)
                segment.gameObject.SetActive(false);
        }
        
        private void StartLowRiskGame()
        {
            if (_isStarted)
                return;
            if (_inventory.CanBuy(_miniGameConfig.LowRiskStats.CostToRun))
            {
                _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
                _currentTierType = MiniGameTierType.Low;
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
            if (_isStarted)
                return;
            if (_inventory.CanBuy(_miniGameConfig.MediumRiskStats.CostToRun))
            {
                _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
                _currentTierType = MiniGameTierType.Medium;
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
            if (_isStarted)
                return;
            if (_inventory.CanBuy(_miniGameConfig.HighRiskStats.CostToRun))
            {
                _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
                _currentTierType = MiniGameTierType.High;
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
            _miniGameSpeedometer.SetSpeed(_speed);
            _isStarted = true;
            _actionButton.gameObject.SetActive(true);
            _lowRiskButton.gameObject.SetActive(false);
            _mediumRiskButton.gameObject.SetActive(false);
            _highRiskButton.gameObject.SetActive(false);
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
            MiniGameEffect selectedEffect = GetMiniGameEffectUnderSelector();
            _soundManager.PlaySoundByType(selectedEffect == null ? GameAudioType.MiniGameLose : GameAudioType.MiniGameWin, 0); 
            //type of the game
            _timerService.AddTimer(1f, () => OnMiniGameEnds?.Invoke(selectedEffect, GetEffectByTier()));
            if (_currentTierType == MiniGameTierType.High)
            {
                _timesWonInARow = selectedEffect == null ? 0 : _timesWonInARow + 1;

                _questService.SetRequirement(RequirementType.WinInThirdTierMiniGameTimes, _timesWonInARow);
            }
        }

        private float GetEffectByTier()
        {
            return _currentTierType switch
            {
                MiniGameTierType.Low => _miniGameConfig.EffectTimeLow,
                MiniGameTierType.Medium => _miniGameConfig.EffectTimeMedium,
                MiniGameTierType.High => _miniGameConfig.EffectTimeHigh,
                _ => 40f
            };
        }
        
        private MiniGameEffect GetMiniGameEffectUnderSelector()
        {
            MiniGameEffect selectedEffect = null;
            _drumShift = _drum.transform.rotation.eulerAngles.z;
            foreach (Segment segment in _segments)
            {
                if ((segment.StartAngle - _drumShift) % FULL_CIRCLE_ANGLE <= -_miniGameConfig.AdditionalAngle && 
                    (segment.StartAngle + segment.Angle - _drumShift) % FULL_CIRCLE_ANGLE >= _miniGameConfig.AdditionalAngle)
                {
                    _isFirstRun = false;
                    selectedEffect = segment.MiniGameEffect;
                    break;
                }
            }
            return selectedEffect;
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
            if (_isFirstRun) 
                return _miniGameConfig.FirstEffect;
            allowedEffects = Shuffle(allowedEffects);
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
        
        private List<MiniGameEffect> Shuffle(List<MiniGameEffect> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (list[n], list[k]) = (list[k], list[n]); // Swap элементов
            }

            return list;
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
            {
                _drum.Rotate(0, 0, _speed * Time.deltaTime);
            }
            
            var effect = GetMiniGameEffectUnderSelector();
            _output.text = effect == null ? "" : $"{effect.BuffType.ToString()}\n{effect.Value} %";
        }
        
        public void SetPaused(bool isPaused) => 
            _isPaused = isPaused;
    }

    internal enum MiniGameTierType
    {
        Low,
        Medium,
        High
    }
}
