using System;
using Farm.Enums;
using Farm.Gameplay.Configs;
using Farm.Gameplay.Definitions;
using Farm.Gameplay.Repositories;
using Farm.Interface;
using Farm.Interface.Popups;
using Farm.Utils.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Farm.Gameplay.Capsules
{
    public class Capsule : MonoBehaviour
    {
        [SerializeField] private Image _growProgress;
        [SerializeField] private SpriteRenderer _embryoImage;
        [SerializeField] private TMP_Text _info;
        [SerializeField] private CapsuleEnergyCost _capsuleEnergyCost;
        [SerializeField] private int _capsuleNumber;
        
        [Inject] private PopupManager _popupManager;
        [Inject] private EmbryoRepository _embryoRepository;
        [Inject] private TimerService _timerService;
        [Inject] private FeedMediator _feedMediator;
        [Inject] private InventoryUI _inventory;
        [Inject] private CapsuleConfig _capsuleConfig;
        private TimerHandle _embryoTimer;
        private EmbryoDefinition _embryo;
        private int _currentTier;
        private bool _isOwn;
        private bool _isFeedReady;
        
        public EmbryoDefinition Embryo => _embryo;
        public event Action OnEmbryoStateChanged;

        public void StartEmbryoProcess()
        {
            _embryo = GetRandomEmbryo();

            _embryoImage.gameObject.SetActive(true);
            _embryoImage.sprite = _embryo.Image;
            
            OnEmbryoStateChanged?.Invoke();
            float timeToGrow = CalculateTimeToGrow();

            _embryoTimer = _timerService.AddTimer(timeToGrow, EmbryoGrewUp);
            _growProgress.gameObject.SetActive(true);
        }

        private float CalculateTimeToGrow() =>
            _embryo.TimeToGrowth;

        private void EmbryoGrewUp()
        {
            _isFeedReady = true;
            _growProgress.gameObject.SetActive(false);
            _info.gameObject.SetActive(true);
        }

        private EmbryoDefinition GetRandomEmbryo()
        {
            float totalChance = 0;
            totalChance += _capsuleConfig.BaseHumanChance;
            float humanProcChance = totalChance;
            totalChance += _capsuleConfig.BaseAnimalChance;
            float animalProcChance = totalChance;
            totalChance += _capsuleConfig.BaseFishChance;

            var chance = Random.Range(0, totalChance);

            if (chance < humanProcChance)
                return _embryoRepository.GetEmbryoByType(EmbryoType.Human);
            
            if (chance < animalProcChance)
                return _embryoRepository.GetEmbryoByType(EmbryoType.Animal);
            
            return _embryoRepository.GetEmbryoByType(EmbryoType.Fish);
        }
        
        private void BuyCapsule() => 
            _isOwn = true;

        private void Awake()
        {
            _growProgress.gameObject.SetActive(false);
            _info.gameObject.SetActive(false);
            _embryoImage.gameObject.SetActive(false);
            
            _capsuleEnergyCost.OnBoughtSuccess += BuyCapsule;

            if (_capsuleConfig.CapsuleCosts[_capsuleNumber] == 0)
                _isOwn = true;
            else
                _capsuleEnergyCost.Initialize(_capsuleConfig.CapsuleCosts[_capsuleNumber]);
        }

        private void Update()
        {
            if (_embryo != null)
                _growProgress.fillAmount = 1 - _embryoTimer.Progress;
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (_isOwn)
            {
                if (_isFeedReady)
                {
                    FeedTheOldOne();
                }
                else
                {
                    _popupManager.OpenCapsule(this);
                }
            }
        }
        
        private void FeedTheOldOne()
        {
            _info.gameObject.SetActive(false);
            _isFeedReady = false;
            _feedMediator.FeedTheOldOne(CalculateFeedAmount());
            _inventory.CurrentEnergy += _embryo.EnergyValue;
            _embryo = null;
            _embryoImage.gameObject.SetActive(false);
            OnEmbryoStateChanged?.Invoke();
        }

        private int CalculateFeedAmount() => 
            _embryo.EnergyValue;

        private void OnDestroy()
        {
            _capsuleEnergyCost.OnBoughtSuccess -= BuyCapsule;
        }
        
        private void MethodForTesting(){}
    }
}
