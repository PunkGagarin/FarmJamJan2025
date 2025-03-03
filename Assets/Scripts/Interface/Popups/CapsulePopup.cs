using System;
using System.Collections.Generic;
using Audio;
using Farm.Enums;
using Farm.Gameplay;
using Farm.Gameplay.Capsules;
using Farm.Gameplay.Configs;
using Farm.Gameplay.Configs.MiniGame;
using Farm.Gameplay.Configs.UpgradeModules;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Farm.Interface.Popups
{
    public class CapsulePopup : Popup
    {
        private static readonly int InstantFill = Animator.StringToHash("InstantFill");
        private static readonly int Fill = Animator.StringToHash("Fill");
        private static readonly int IsEmpty = Animator.StringToHash("IsEmpty");

        [SerializeField] private Animator _animator;
        [SerializeField] private Image _embryo;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _additionalCloseButton;
        [SerializeField] private Button _createEmbryoButton;
        [SerializeField] private TMP_Text _cost;
        [SerializeField] private TMP_Text _createEmbryoButtonText;
        [SerializeField] private CapsuleInfo _capsuleInfo;
        [Header("Tier buttons")]
        [SerializeField] private Button _minusTier;
        [SerializeField] private Button _plusTier;
        [Header("Module Slots")] 
        [SerializeField] private List<CapsuleSlotProvider> _slotProviders;

        [Inject] private CapsuleConfig _capsuleConfig;
        [Inject] private EmbryoConfig _embryoConfig;
        [Inject] private InventoryUI _inventory;
        [Inject] private MiniGameEffectsMediator _miniGameEffectsMediator;
        [Inject] private SoundManager _sfxManager;

        private int _selectedTier;
        private Capsule _capsule;
        private Embryo _selectedEmbryo;
        private MiniGameEffect _miniGameEffect;
        private bool _isTutorial = true;

        private const float PERCENT_VALUE = 100f;
        private const string CREATE_EMBRYO_TEXT = "Create embryo Tier ";

        private int Cost => _embryoConfig.EmbryoTiers[_selectedTier].BaseCost;
        private bool CanBuy => _inventory.CanBuy(Cost);
        
        public static event Action OnTutorialCapsuleOpened;
        public static event Action OnTutorialCapsuleClosed;
        public static event Action OnTutorialAnimationCompleted;

        public void Initialize(Capsule capsule)
        {
            _capsule = capsule;
            _selectedEmbryo = capsule.Embryo;
            _createEmbryoButton.interactable = _capsule.Embryo == null;
            _createEmbryoButton.onClick.AddListener(OnBuy);
            _closeButton.onClick.AddListener(Close);
            _additionalCloseButton.onClick.AddListener(Close);
            _capsule.OnEmbryoStateChanged += UpdateEmbryoView;
            _selectedTier = 0;
            _additionalCloseButton.gameObject.SetActive(_capsule.Embryo != null);

            UpdatePopupInfo();

            UpdateButtonsInfo();

            UpdateEmbryoView(capsule.CurrentEmbryoState);

            UpdateModulesSots();

            if (_isTutorial)
            {
                _closeButton.interactable = false;
                _additionalCloseButton.interactable = false;
            }

            OnTutorialCapsuleOpened?.Invoke();
        }

        public override void Open(bool withPause)
        {
            base.Open(withPause);
            
            _animator.SetBool(IsEmpty, _capsule.Embryo == null);
            
            if (_capsule.Embryo != null)
                _animator.SetTrigger(InstantFill);
        }

        private void TutorialAnimationComplete()
        {
            _isTutorial = false;
            _closeButton.interactable = true;
            _additionalCloseButton.interactable = true;
            OnTutorialAnimationCompleted?.Invoke();
        }

        private void UpdateModulesSots()
        {
            for (var i = 0; i < _slotProviders.Count; i++)
                _slotProviders[i].SetSlot(_capsule.CapsuleSlots[i]);
        }

        private void UpdatePopupInfo()
        {
            _capsuleInfo.SetCapsuleInfo(
                _capsule.Tier,
                Mathf.RoundToInt(UpgradeModuleUtils.ApplyStatsWithType(_capsuleConfig.BaseHumanChance,
                    _capsule.CapsuleSlots, ModuleCharacteristicType.HumanRoll)),
                Mathf.RoundToInt(UpgradeModuleUtils.ApplyStatsWithType(_capsuleConfig.BaseAnimalChance,
                    _capsule.CapsuleSlots, ModuleCharacteristicType.AnimalRoll)),
                Mathf.RoundToInt(UpgradeModuleUtils.ApplyStatsWithType(_capsuleConfig.BaseFishChance,
                    _capsule.CapsuleSlots, ModuleCharacteristicType.FishRoll))
            );

            _capsuleInfo.SetEmbryoInfo(_selectedEmbryo);
            _capsuleInfo.SetModulesInfo(_capsule.CapsuleSlots);
        }

        protected override void Close()
        {
            _closeButton.onClick.RemoveListener(Close);
            _additionalCloseButton.onClick.RemoveListener(Close);
            _capsule.OnEmbryoStateChanged -= UpdateEmbryoView;
            _createEmbryoButton.interactable = false;
            _sfxManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
            base.Close();
            OnTutorialCapsuleClosed?.Invoke();
        }

        private void PlusTier()
        {
            _selectedTier = Mathf.Clamp(_selectedTier + 1, 0, _capsule.Tier);
            UpdateButtonsInfo();
            UpdatePopupInfo();
            _sfxManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
        }

        private void MinusTier()
        {
            _selectedTier = Mathf.Clamp(_selectedTier - 1, 0, _capsule.Tier);
            UpdateButtonsInfo();
            UpdatePopupInfo();
            _sfxManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
        }

        private void StartEmbryoProcess()
        {
            _animator.SetBool(IsEmpty, false);
            _closeButton.gameObject.SetActive(true);
            _additionalCloseButton.gameObject.SetActive(true);
            ApplyMiniGameEffects();
            ApplyModulesEffects();

            UpdatePopupInfo();
            _animator.SetTrigger(Fill);
            _capsule.StartEmbryoProcess(_selectedEmbryo);
            _sfxManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
        }

        private void ApplyMiniGameEffects()
        {
            if (_miniGameEffect == null)
                return;

            switch (_miniGameEffect.BuffType)
            {
                case BuffType.UpdateTier:
                    int newTier = _selectedTier + _miniGameEffect.Value;
                    if (newTier < 0 || newTier >= _embryoConfig.EmbryoTiers.Count)
                        break;

                    _selectedEmbryo.EnergyValue = _embryoConfig.EmbryoTiers[newTier].BaseEnergyAmount;
                    _selectedEmbryo.TimeToGrowth = _embryoConfig.EmbryoTiers[newTier].BaseGrowthSpeed;
                    _selectedEmbryo.StarvationValue = _embryoConfig.EmbryoTiers[newTier].BaseFoodAmount;
                    break;
                case BuffType.GrowthSpeed:
                    _selectedEmbryo.TimeToGrowth += _selectedEmbryo.TimeToGrowth * (_miniGameEffect.Value / PERCENT_VALUE);
                    break;
                case BuffType.EnergyAmount:
                    _selectedEmbryo.EnergyValue += Mathf.RoundToInt(_selectedEmbryo.EnergyValue * (_miniGameEffect.Value / PERCENT_VALUE));
                    break;
                case BuffType.SatietyAmount:
                    _selectedEmbryo.StarvationValue += Mathf.RoundToInt(_selectedEmbryo.StarvationValue * (_miniGameEffect.Value / PERCENT_VALUE));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ApplyModulesEffects()
        {
            _selectedEmbryo.EnergyValue =
                Mathf.RoundToInt(UpgradeModuleUtils.ApplyStatsWithType(_selectedEmbryo.EnergyValue,
                    _capsule.CapsuleSlots, ModuleCharacteristicType.EnergyCharge));
            _selectedEmbryo.TimeToGrowth =
                Mathf.RoundToInt(UpgradeModuleUtils.ApplyStatsWithType(_selectedEmbryo.TimeToGrowth,
                    _capsule.CapsuleSlots, ModuleCharacteristicType.GrowthSpeed));
            _selectedEmbryo.StarvationValue =
                Mathf.RoundToInt(UpgradeModuleUtils.ApplyStatsWithType(_selectedEmbryo.StarvationValue,
                    _capsule.CapsuleSlots, ModuleCharacteristicType.Satiety));
        }

        private void CreateEmbryo()
        {
            _sfxManager.PlaySoundByType(GameAudioType.EmbryoCreating, 0);
            _createEmbryoButton.onClick.RemoveListener(OnBuy);

            _createEmbryoButton.interactable = false;
            EmbryoType type = GetEmbryoType();
            var embryoTier = _embryoConfig.EmbryoTiers[_selectedTier];
            _selectedEmbryo = new Embryo(type, embryoTier.BaseFoodAmount, embryoTier.BaseGrowthSpeed, embryoTier.BaseEnergyAmount);
            UpdatePopupInfo();
            StartEmbryoProcess();
        }

        private EmbryoType GetEmbryoType()
        {
            float humanChance = GetChance(EmbryoType.Human);
            float animalChance = GetChance(EmbryoType.Animal);
            float fishChance = GetChance(EmbryoType.Fish);

            float totalChance = 0;
            totalChance += humanChance;
            var humanProcChance = totalChance;
            totalChance += animalChance;
            var animalProcChance = totalChance;
            totalChance += fishChance;

            var chance = Random.Range(0, totalChance);

            if (chance < humanProcChance)
                return EmbryoType.Human;

            if (chance < animalProcChance)
                return EmbryoType.Animal;

            return EmbryoType.Fish;
        }

        private float GetChance(EmbryoType type)
        {
            float chance = type switch
            {
                EmbryoType.Human => Mathf.RoundToInt(UpgradeModuleUtils.ApplyStatsWithType(
                    _capsuleConfig.BaseHumanChance, _capsule.CapsuleSlots, ModuleCharacteristicType.HumanRoll)),
                EmbryoType.Animal => Mathf.RoundToInt(UpgradeModuleUtils.ApplyStatsWithType(
                    _capsuleConfig.BaseAnimalChance, _capsule.CapsuleSlots, ModuleCharacteristicType.AnimalRoll)),
                EmbryoType.Fish => Mathf.RoundToInt(UpgradeModuleUtils.ApplyStatsWithType(_capsuleConfig.BaseFishChance,
                    _capsule.CapsuleSlots, ModuleCharacteristicType.FishRoll)),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return chance;
        }

        private void UpdateEmbryoView(EmbryoStates newEmbryoState)
        {
            if (_capsule.Embryo == null)
            {
                _embryo.sprite = null;
                return;
            }
            
            _embryo.sprite = newEmbryoState switch
            {
                EmbryoStates.Empty => null,
                EmbryoStates.Growing => _embryoConfig.GetUISprite(_capsule.Embryo.EmbryoType),
                EmbryoStates.End => _embryoConfig.GetUISpriteEnd(_capsule.Embryo.EmbryoType),
                _ => throw new ArgumentOutOfRangeException(nameof(newEmbryoState), newEmbryoState, null)
            };
        }
        [UsedImplicitly]
        public void MouseEnter()
        {
            if (Cost == 0)
                return;
            if (CanBuy)
                _inventory.ShowCanBuy();
            else
                _inventory.ShowCanNotBuy();
        }

        [UsedImplicitly]
        public void MouseExit()
        {
            _inventory.ResetColor();
        }

        private void OnBuy()
        {
            if (_selectedTier == 0)
            {
                CreateEmbryo();
            }
            else
            {
                if (CanBuy)
                {
                    _inventory.CurrentEnergy -= Cost;
                    _inventory.ResetColor();
                    CreateEmbryo();
                }
                else
                {
                    _inventory.ShakeCanNotBuy();
                }
            }
        }

        private void UpdateButtonsInfo()
        {
            _createEmbryoButtonText.text = $"{CREATE_EMBRYO_TEXT}{_selectedTier}";
            _cost.gameObject.SetActive(Cost > 0);
            _cost.text = Cost.ToString();
        }

        private void MiniGameEffectChanged(MiniGameEffect miniGameEffect) => 
            _miniGameEffect = miniGameEffect;

        private void OnAnyModuleChanged(CapsuleSlot obj)
        {
            _capsule.CapsuleSlots.ForEach(slot =>
            {
                if (slot == obj)
                {
                    UpdatePopupInfo();
                }
            });
        }

        private void Awake()
        {
            _plusTier.onClick.AddListener(PlusTier);
            _minusTier.onClick.AddListener(MinusTier);
            _miniGameEffectsMediator.OnEffectChanged += MiniGameEffectChanged;
            CapsuleSlot.OnAnyModuleChanged += OnAnyModuleChanged;
        }

        private void OnDestroy()
        {
            _plusTier.onClick.RemoveListener(PlusTier);
            _minusTier.onClick.RemoveListener(MinusTier);
            _miniGameEffectsMediator.OnEffectChanged -= MiniGameEffectChanged;
            CapsuleSlot.OnAnyModuleChanged -= OnAnyModuleChanged;
        }
    }
}