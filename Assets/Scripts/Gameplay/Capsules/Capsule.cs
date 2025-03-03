using System;
using System.Collections.Generic;
using Audio;
using DG.Tweening;
using Farm.Gameplay.Configs;
using Farm.Gameplay.Configs.UpgradeModules;
using Farm.Interface;
using Farm.Interface.Popups;
using Farm.Utils.Timer;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Zenject;

namespace Farm.Gameplay.Capsules
{
    public class Capsule : MonoBehaviour
    {
        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int InstantOpen = Animator.StringToHash("InstantOpen");
        private static readonly int Feed = Animator.StringToHash("Feed");
        private static readonly int Fill = Animator.StringToHash("Fill");
        
        [SerializeField] private Animator _animator;
        [Header("Components")]
        [SerializeField] private SpriteRenderer _capsuleSprite;
        [SerializeField] private Image _growProgress;
        [SerializeField] private SpriteRenderer _embryoImage;
        [SerializeField] private TMP_Text _info;
        [SerializeField] private CapsuleEnergyCost _capsuleEnergyCost;
        [SerializeField] private int _capsuleNumber;
        [SerializeField] private List<CapsuleSlot> _capsuleSlots;
        [SerializeField] private Image _canBuyImage;

        [Inject] private PopupManager _popupManager;
        [Inject] private TimerService _timerService;
        [Inject] private FeedMediator _feedMediator;
        [Inject] private InventoryUI _inventory;
        [Inject] private CapsuleConfig _capsuleConfig;
        [Inject] private UpgradeModuleConfig _upgradeModuleConfig;
        [Inject] private EmbryoConfig _embryoConfig;
        [Inject] private SoundManager _sfxManager;

        private TimerHandle _embryoTimer;
        private Light2D _light2D;
        private int _tier;
        private bool _isOwn;
        private bool _animationInProgress;
        private bool _isFeedReady;
        private const float ENABLE_LIGHT = 6.7f;
        private const float DISABLE_LIGHT = 0f;

        public List<CapsuleSlot> CapsuleSlots => _capsuleSlots;
        public Embryo Embryo { get; private set; }
        public int Tier => _tier;
        public bool IsOwn => _isOwn;
        public int Cost => _capsuleEnergyCost.Cost;
        public EmbryoStates CurrentEmbryoState { get; private set; }

        public event Action<EmbryoStates> OnEmbryoStateChanged;
        public static event Action OnCapsuleBought;
        public static event Action OnCapsuleUpgrade;
        public static event Action OnTutorialCapsuleEmbryoReleased;

        public void StartEmbryoProcess(Embryo embryo)
        {
            Embryo = embryo;

            _animator.SetTrigger(Fill);

            CurrentEmbryoState = EmbryoStates.Growing;
            OnEmbryoStateChanged?.Invoke(CurrentEmbryoState);
            ApplyModulesToGrowthSpeed();
            _embryoTimer = _timerService.AddTimer(Embryo.TimeToGrowth, EmbryoGrewUp);
            _growProgress.gameObject.SetActive(true);
            _capsuleSlots.ForEach(slot => slot.IsCapsuleInGrowthProcess = true);
        }
        
        public void OpenCapsule()
        {
            _animator.SetTrigger(Open);
        }
        
        public void ShowTutorialCanBuy()
        {
            _canBuyImage.gameObject.SetActive(true);
            _canBuyImage.DOColor(Color.yellow, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        private void ApplyModulesToGrowthSpeed()
        {
            Embryo.TimeToGrowth = UpgradeModuleUtils.ApplyStatsWithType(Embryo.TimeToGrowth, _capsuleSlots,
                ModuleCharacteristicType.GrowthSpeed);
        }

        private void EmbryoGrewUp()
        {
            CurrentEmbryoState = EmbryoStates.End;
            OnEmbryoStateChanged?.Invoke(CurrentEmbryoState);
            _embryoTimer = null;
            _isFeedReady = true;
            _growProgress.gameObject.SetActive(false);
            _info.gameObject.SetActive(true);
            _capsuleSlots.ForEach(slot => slot.IsCapsuleInGrowthProcess = false);
        }
        
        private void BuyCapsule()
        {
            _animator.SetTrigger(Open);
            UpdateOwnership();
            _sfxManager.PlaySoundByType(GameAudioType.CapsuleAppearing, 0);
            _timerService.AddTimer(1.4f, () => _light2D.intensity = ENABLE_LIGHT);
        }
        
        private void Awake()
        {
            _growProgress.gameObject.SetActive(false);
            _info.gameObject.SetActive(false);
            OnEmbryoStateChanged += UpdateView;
            _light2D = GetComponentInChildren<Light2D>();
            _light2D.intensity = DISABLE_LIGHT;
            SetupCapsule();
        }
        
        private void UpdateView(EmbryoStates newEmbryoState)
        {
            if (Embryo == null)
            {
                _embryoImage.sprite = null;
                return;
            }
            
            _embryoImage.sprite = newEmbryoState switch
            {
                EmbryoStates.Empty => null,
                EmbryoStates.Growing => _embryoConfig.GetSpriteStart(Embryo.EmbryoType),
                EmbryoStates.End => _embryoConfig.GetSpriteEnd(Embryo.EmbryoType),
                _ => throw new ArgumentOutOfRangeException(nameof(newEmbryoState), newEmbryoState, null)
            };
        }
        
        private void SetupCapsule()
        {
            if (_capsuleConfig.CapsuleCosts[_capsuleNumber] == 0)
            {
                UpdateOwnership();
                _light2D.intensity = ENABLE_LIGHT;
            }
            else
            {
                _capsuleEnergyCost.OnBoughtSuccess += BuyCapsule;
                _capsuleEnergyCost.UpdateInfo(_capsuleConfig.CapsuleCosts[_capsuleNumber], false);
            }
        }
        
        private void UpdateOwnership()
        {
            _canBuyImage.gameObject.SetActive(false);
            _canBuyImage.DOKill();
            
            CurrentEmbryoState = EmbryoStates.Empty;
            _isOwn = true;
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

            if (!_isOwn || _animationInProgress)
                return;

            if (_isFeedReady)
            {
                _animationInProgress = true;
                _isFeedReady = false;
                _info.gameObject.SetActive(false);
                _animator.SetTrigger(Feed);
            }
            else
            {
                _popupManager.OpenCapsule(this);
                _sfxManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
            }
        }

        private void FeedTheOldOne()
        {
            _sfxManager.PlayRandomSoundByTypeWithRandomChance(GameAudioType.FeedingAction, 0, true);
            _feedMediator.FeedTheOldOne(Embryo.StarvationValue, Embryo.EmbryoType);
            _inventory.CurrentEnergy += Embryo.EnergyValue;
            Embryo = null;

            CurrentEmbryoState = EmbryoStates.Empty;
            OnEmbryoStateChanged?.Invoke(CurrentEmbryoState);
        }

        private void CompleteAnimation()
        {
            OnTutorialCapsuleEmbryoReleased?.Invoke();
            _animationInProgress = false;
        }

        private void OnDestroy()
        {
            _capsuleEnergyCost.OnBoughtSuccess -= BuyCapsule;
            _capsuleEnergyCost.OnBoughtSuccess -= UpgradeCapsule;
            OnEmbryoStateChanged -= UpdateView;
        }
    }
}