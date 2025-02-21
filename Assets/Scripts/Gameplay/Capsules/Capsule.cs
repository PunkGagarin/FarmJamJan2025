using System;
using System.Collections.Generic;
using Farm.Enums;
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

namespace Farm.Gameplay
{
    public class Capsule : MonoBehaviour
    {
        [SerializeField] private Image _growProgress;
        [SerializeField] private SpriteRenderer _embryoImage;
        [SerializeField] private TMP_Text _info;
        [SerializeField] private CapsuleEnergyCost _capsuleEnergyCost;
        [SerializeField] private CapsuleDefinition _capsuleDefinition;
        [SerializeField] private List<CapsuleSlotProvider> _capsuleSlots;

        [Inject] private PopupManager _popupManager;
        [Inject] private EmbryoRepository _embryoRepository;
        [Inject] private TimerService _timerService;
        [Inject] private FeedMediator _feedMediator;
        [Inject] private InventoryUI _inventory;
        private TimerHandle _embryoTimer;
        private EmbryoDefinition _embryo;
        private bool _isOwn;
        private bool _isFeedReady;

        public EmbryoDefinition Embryo => _embryo;
        public List<CapsuleSlotProvider> CapsuleSlots => _capsuleSlots;

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

        private float CalculateTimeToGrow()
        {
            return _embryo.TimeToGrowth / _capsuleDefinition.GrowthSpeed;
        }

        private void EmbryoGrewUp()
        {
            _isFeedReady = true;
            _growProgress.gameObject.SetActive(false);
            _info.gameObject.SetActive(true);
        }

        private EmbryoDefinition GetRandomEmbryo()
        {
            float totalChance = 0;
            totalChance += _capsuleDefinition.BaseEmbryoChances.HumanChance;
            float humanProcChance = totalChance;
            totalChance += _capsuleDefinition.BaseEmbryoChances.AnimalChance;
            float animalProcChance = totalChance;
            totalChance += _capsuleDefinition.BaseEmbryoChances.FishChance;

            var chance = Random.Range(0, totalChance);

            if (chance < humanProcChance)
                return _embryoRepository.GetEmbryoByType(EmbryoType.Human);

            if (chance < animalProcChance)
                return _embryoRepository.GetEmbryoByType(EmbryoType.Animal);

            return _embryoRepository.GetEmbryoByType(EmbryoType.Fish);
        }

        private void BuyCapsule() => UpdateOwnership();

        private void Awake()
        {
            _growProgress.gameObject.SetActive(false);
            _info.gameObject.SetActive(false);
            _embryoImage.gameObject.SetActive(false);

            _capsuleEnergyCost.OnBoughtSuccess += BuyCapsule;

            if (_capsuleDefinition.CostToUnlock == 0)
            {
                UpdateOwnership();
            }
            else
            {
                _capsuleEnergyCost.Initialize(_capsuleDefinition);
            }

            CapsuleSlotProvider.OnAnyModuleChanged += OnModuleChanged;
        }

        private void UpdateOwnership()
        {
            _isOwn = true;
            _capsuleSlots.ForEach(slot => slot.IsOwn = true);
        }

        private void OnModuleChanged(CapsuleSlotProvider slot)
        {
            _capsuleSlots.ForEach(capsuleSlot =>
            {
                if (capsuleSlot == slot)
                {
                    //todo
                    Debug.Log($"OnModuleChanged {name}, {slot.name}");
                }
            });
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

        private int CalculateFeedAmount()
        {
            return _embryo.EnergyValue;
        }

        private void OnDestroy()
        {
            _capsuleEnergyCost.OnBoughtSuccess -= BuyCapsule;
            CapsuleSlotProvider.OnAnyModuleChanged -= OnModuleChanged;
        }
    }
}