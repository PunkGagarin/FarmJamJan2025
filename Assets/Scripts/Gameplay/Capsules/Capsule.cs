using System;
using System.Collections.Generic;
using Farm.Gameplay.Configs;
using Farm.Gameplay.Configs.MiniGame;
using Farm.Gameplay.Configs.UpgradeModules;
using Farm.Interface;
using Farm.Interface.Popups;
using Farm.Utils.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Farm.Gameplay.Capsules
{
    public class Capsule : MonoBehaviour
    {
        [SerializeField] private Image _growProgress;
        [SerializeField] private SpriteRenderer _embryoImage;
        [SerializeField] private TMP_Text _info;
        [SerializeField] private CapsuleEnergyCost _capsuleEnergyCost;
        [SerializeField] private int _capsuleNumber;
        [SerializeField] private List<CapsuleSlotProvider> _capsuleSlots;
        
        [Inject] private PopupManager _popupManager;
        [Inject] private TimerService _timerService;
        [Inject] private FeedMediator _feedMediator;
        [Inject] private InventoryUI _inventory;
        [Inject] private CapsuleConfig _capsuleConfig;
        [Inject] private UpgradeModuleConfig _upgradeModuleConfig;
        [Inject] private EmbryoConfig _embryoConfig;
        
        private TimerHandle _embryoTimer;
        private int _tier;
        private bool _isOwn;
        private bool _isFeedReady;

        public List<CapsuleSlotProvider> CapsuleSlots => _capsuleSlots;
        public Embryo Embryo { get; private set; }
        public int Tier => _tier;
        public bool IsOwn => _isOwn;

        private const float PERCENT_VALUE = 100f;
        
        public event Action OnEmbryoStateChanged;
        public static event Action OnCapsuleBought;
        public static event Action OnCapsuleUpgrade;

        public void StartEmbryoProcess(Embryo embryo)
        {
            Embryo = embryo;

            _embryoImage.gameObject.SetActive(true);
            _embryoImage.sprite = Embryo.Image;

            OnEmbryoStateChanged?.Invoke();

            _embryoTimer = _timerService.AddTimer(Embryo.TimeToGrowth, EmbryoGrewUp);
            _growProgress.gameObject.SetActive(true);
            _capsuleSlots.ForEach(slot => slot.IsCapsuleInGrowthProcess = true);
        }

        private void EmbryoGrewUp()
        {
            _embryoTimer = null;
            _isFeedReady = true;
            _growProgress.gameObject.SetActive(false);
            _info.gameObject.SetActive(true);
            _capsuleSlots.ForEach(slot => slot.IsCapsuleInGrowthProcess = false);
        }

        private void BuyCapsule() => 
            UpdateOwnership();

        private void Awake()
        {
            _growProgress.gameObject.SetActive(false);
            _info.gameObject.SetActive(false);
            _embryoImage.gameObject.SetActive(false);

            SetupCapsule();

            CapsuleSlotProvider.OnAnyModuleChanged += OnModuleChanged;
        }


        private void SetupCapsule()
        {
            if (_capsuleConfig.CapsuleCosts[_capsuleNumber] == 0)
            {
                UpdateOwnership();
            }
            else
            {
                _capsuleEnergyCost.OnBoughtSuccess += BuyCapsule;
                _capsuleEnergyCost.UpdateInfo(_capsuleConfig.CapsuleCosts[_capsuleNumber], false);
            }
        }

        private void UpdateOwnership()
        {
            _isOwn = true;
            _capsuleSlots.ForEach(slot => slot.IsOwn = true);
            _capsuleEnergyCost.UpdateInfo(_capsuleConfig.UpgradeCost, true);
            OnCapsuleBought?.Invoke();

            _capsuleEnergyCost.OnBoughtSuccess -= BuyCapsule;
            _capsuleEnergyCost.OnBoughtSuccess += UpgradeCapsule;
        }
        
        private void UpgradeCapsule()
        {
            _tier++;
            OnCapsuleUpgrade?.Invoke();
            if (_tier >= _embryoConfig.EmbryoTiers.Count - 1)
            {
                _capsuleEnergyCost.OnBoughtSuccess -= UpgradeCapsule;
                _capsuleEnergyCost.gameObject.SetActive(false);
            }
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
            if (Embryo != null && _embryoTimer != null)
                _growProgress.fillAmount = 1 - _embryoTimer.Progress;
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (!_isOwn)
                return;

            if (_isFeedReady)
                FeedTheOldOne();
            else
                _popupManager.OpenCapsule(this);
        }

        private void FeedTheOldOne()
        {
            _info.gameObject.SetActive(false);
            _isFeedReady = false;
            _feedMediator.FeedTheOldOne(Embryo.StarvationValue, Embryo.EmbryoType);
            _inventory.CurrentEnergy += Embryo.EnergyValue;
            Embryo = null;
            _embryoImage.gameObject.SetActive(false);
            OnEmbryoStateChanged?.Invoke();
        }

        private void OnDestroy()
        {
            _capsuleEnergyCost.OnBoughtSuccess -= BuyCapsule;
            _capsuleEnergyCost.OnBoughtSuccess -= UpgradeCapsule;
            CapsuleSlotProvider.OnAnyModuleChanged -= OnModuleChanged;
        }
    }
}