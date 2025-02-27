using System;
using System.Collections.Generic;
using Audio;
using Farm.Gameplay.Configs;
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
        [Header("Resources")]
        [SerializeField] private Color _closedColor;
        [SerializeField] private Color _openedColor;
        [Header("Components")]
        [SerializeField] private SpriteRenderer _capsuleSprite;
        [SerializeField] private Image _growProgress;
        [SerializeField] private SpriteRenderer _embryoImage;
        [SerializeField] private TMP_Text _info;
        [SerializeField] private CapsuleEnergyCost _capsuleEnergyCost;
        [SerializeField] private int _capsuleNumber;
        [SerializeField] private List<CapsuleSlot> _capsuleSlots;

        [Inject] private PopupManager _popupManager;
        [Inject] private TimerService _timerService;
        [Inject] private FeedMediator _feedMediator;
        [Inject] private InventoryUI _inventory;
        [Inject] private CapsuleConfig _capsuleConfig;
        [Inject] private UpgradeModuleConfig _upgradeModuleConfig;
        [Inject] private EmbryoConfig _embryoConfig;
        [Inject] private SoundManager _sfxManager;

        private TimerHandle _embryoTimer;
        private int _tier;
        private bool _isOwn;
        private bool _isFeedReady;

        public List<CapsuleSlot> CapsuleSlots => _capsuleSlots;
        public Embryo Embryo { get; private set; }
        public int Tier => _tier;
        public bool IsOwn => _isOwn;

        public event Action OnEmbryoStateChanged;
        public static event Action OnCapsuleBought;
        public static event Action OnCapsuleUpgrade;

        public void StartEmbryoProcess(Embryo embryo)
        {
            Embryo = embryo;

            _embryoImage.gameObject.SetActive(true);
            _embryoImage.sprite = Embryo.Image;

            OnEmbryoStateChanged?.Invoke();
            ApplyModulesToGrowthSpeed();
            _embryoTimer = _timerService.AddTimer(Embryo.TimeToGrowth, EmbryoGrewUp);
            _growProgress.gameObject.SetActive(true);
            _capsuleSlots.ForEach(slot => slot.IsCapsuleInGrowthProcess = true);
        }

        private void ApplyModulesToGrowthSpeed()
        {
            Embryo.TimeToGrowth = UpgradeModuleUtils.ApplyStatsWithType(Embryo.TimeToGrowth, _capsuleSlots,
                ModuleCharacteristicType.GrowthSpeed);
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
            _capsuleSprite.color = _closedColor;

            SetupCapsule();
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
            _capsuleSprite.color = _openedColor;
            _capsuleSlots.ForEach(slot => slot.IsOwn = true);
            _capsuleEnergyCost.UpdateInfo(_capsuleConfig.UpgradeCost, true);
            OnCapsuleBought?.Invoke();
            _inventory.OnEnergyChanged += _capsuleEnergyCost.CheckCanBuy;
            _capsuleEnergyCost.CheckCanBuy();
            _capsuleEnergyCost.OnBoughtSuccess -= BuyCapsule;
            _capsuleEnergyCost.OnBoughtSuccess += UpgradeCapsule;
        }

        private void UpgradeCapsule()
        {
            _tier++;
            OnCapsuleUpgrade?.Invoke();
            if (_tier >= _embryoConfig.EmbryoTiers.Count - 1)
            {
                _inventory.OnEnergyChanged -= _capsuleEnergyCost.CheckCanBuy;
                _capsuleEnergyCost.OnBoughtSuccess -= UpgradeCapsule;
                _capsuleEnergyCost.gameObject.SetActive(false);
            }
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
            {
                FeedTheOldOne();
                _sfxManager.PlaySoundByType(GameAudioType.FeedingAction, 0);
            }
            else
            {
                _popupManager.OpenCapsule(this);
                _sfxManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
            }
        }

        private void FeedTheOldOne()
        {
            _info.gameObject.SetActive(false);
            _isFeedReady = false;
            _feedMediator.FeedTheOldOne(CalculateFeedAmount(), Embryo.EmbryoType);
            _inventory.CurrentEnergy += CalculateEnergyValue();
            Embryo = null;
            _embryoImage.gameObject.SetActive(false);
            OnEmbryoStateChanged?.Invoke();
        }

        private int CalculateEnergyValue() =>
            Embryo.EnergyValue;

        private int CalculateFeedAmount() =>
            Embryo.StarvationValue;

        private void OnDestroy()
        {
            _capsuleEnergyCost.OnBoughtSuccess -= BuyCapsule;
            _capsuleEnergyCost.OnBoughtSuccess -= UpgradeCapsule;
        }
    }
}