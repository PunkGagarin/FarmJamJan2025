using System;
using System.Linq;
using Audio;
using DG.Tweening;
using Farm.Gameplay.Configs.UpgradeModules;
using Farm.Gameplay.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Interface
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _inventoryRectTransform;
        [SerializeField] private Transform _energyPanel;
        [SerializeField] private TMP_Text _energyAmount;
        [SerializeField] private Transform _inventorySlotsContent;
        [SerializeField] private InventorySlot _inventorySlotPrefab;
        [SerializeField] private Button _buyModuleButton;
        [SerializeField] private Button _openInventoryButton;
        [SerializeField] private TMP_Text _placedSlotsCountText;

        [Inject] private SoundManager _soundManager;
        [Inject] private InventoryConfig _inventoryConfig;
        [Inject] private UpgradeModuleConfig _upgradeModuleConfig;
        [Inject] private QuestProvider _questProvider;

        private InventorySlot[] _inventorySlots;
        private float _currentModuleCost;
        private int _currentEnergy;
        private bool _isInventoryOpen = true;
        private int _moduleBoughtCount = 0;

        public InventorySlot[] InventorySlots => _inventorySlots;
        
        public event Action OnEnergyChanged;

        public int CurrentEnergy
        {
            get => _currentEnergy;
            set
            {
                if (value == _currentEnergy)
                    return;

                _currentEnergy = value;
                _energyAmount.text = _currentEnergy.ToString();
                OnEnergyChanged?.Invoke();
                _questProvider.SetRequirement(RequirementType.Energy, _currentEnergy);
            }
        }

        private void OnBuyUpgradeModuleClicked()
        {
            if (!CanPlaceItem)
            {
                _soundManager.PlaySoundByType(GameAudioType.ActionError, 0);
                ShakeNotEnoughPlace();
                return;
            }

            if (!CanBuy((int)_currentModuleCost))
            {
                _soundManager.PlaySoundByType(GameAudioType.ActionError, 0);
                ShakeCanNotBuy();
                return;
            }

            _soundManager.PlaySoundByType(GameAudioType.ModuleAddedAction, 0);
            BuyUpgradeModule();
        }

        private void BuyUpgradeModule()
        {
            CurrentEnergy -= (int)_currentModuleCost;
            _currentModuleCost += Mathf.Round(_currentModuleCost * _inventoryConfig.ModuleCostMultiplier);
            UpdateBuyModuleButtonText();
            UpgradeModule newModule = _upgradeModuleConfig.GetRandomModule(_moduleBoughtCount++ >= _upgradeModuleConfig.CanBeThreeStatCount);
            Debug.Log($"Module moduleBoughtCount{_moduleBoughtCount}");
            newModule.Stats.ForEach(stat =>
            {
                Debug.Log(stat.ToString());
            });
            SetItem(newModule);
        }

        public void SetItem(UpgradeModule module)
        {
            _inventorySlots.First(slot => slot.UpgradeModule == null).SetModule(module);
            UpdatePlacedSlotsCountText();
        }

        public bool CanPlaceItem => _inventorySlots.Any(slot => slot.UpgradeModule == null);

        private void UpdatePlacedSlotsCountText()
        {
            var placedCount = _inventorySlots.Count(slot => slot.UpgradeModule != null);
            _placedSlotsCountText.text =
                $"{placedCount}/{_inventoryConfig.MaxSlotsCount}";
        }

        private void InitializeSlots()
        {
            _inventorySlots = new InventorySlot[_inventoryConfig.MaxSlotsCount];
            for (var i = 0; i < _inventorySlots.Length; i++)
            {
                _inventorySlots[i] = Instantiate(_inventorySlotPrefab, _inventorySlotsContent);
                _inventorySlots[i].Initialize();
            }

            UpdatePlacedSlotsCountText();
        }

        private void UpdateBuyModuleButtonText()
        {
            var textMeshProUGUI = _buyModuleButton.GetComponentInChildren<TextMeshProUGUI>();
            if (textMeshProUGUI != null)
            {
                textMeshProUGUI.text = $"Buy module  for \n{_currentModuleCost}";
            }
        }

        public void ShakeNotEnoughPlace() =>
            _placedSlotsCountText.transform.DOShakePosition(_inventoryConfig.ShakeDuration, Vector3.one * _inventoryConfig.ShakePower);

        public bool CanBuy(int cost) =>
            _currentEnergy >= cost;

        public void ShowCanBuy() =>
            _energyAmount.color = _inventoryConfig.CanBuyColor;

        public void ShowCanNotBuy() =>
            _energyAmount.color = _inventoryConfig.CanNotBuyColor;

        public void ResetColor() =>
            _energyAmount.color = _inventoryConfig.RegularColor;

        public void ShakeCanNotBuy() =>
            _energyPanel.DOShakePosition(_inventoryConfig.ShakeDuration, Vector3.one * _inventoryConfig.ShakePower);

        private void ToggleOpenAnimation()
        {
            var closedX = 0;
            var openedX = -_inventoryRectTransform.rect.width;
            _inventoryRectTransform.DOAnchorPosX(endValue: _isInventoryOpen ? closedX : openedX, duration: 0.5f);
            _isInventoryOpen = !_isInventoryOpen;
            _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
        }

        private void CollectQuestInfo()
        {
            _questProvider.SetRequirement(RequirementType.Energy, _currentEnergy);
        }
        
        private void OnQuestCompleted(int questEnergyReward)
        {
            CurrentEnergy += questEnergyReward;
        }
        
        private void Awake()
        {
            CurrentEnergy = _inventoryConfig.StartEnergy;
            InitializeSlots();
            _currentModuleCost = _inventoryConfig.BaseModuleCost;
            UpdateBuyModuleButtonText();
            _buyModuleButton.onClick.AddListener(OnBuyUpgradeModuleClicked);
            _openInventoryButton.onClick.AddListener(ToggleOpenAnimation);
            InventorySlot.OnModuleChanged += UpdatePlacedSlotsCountText;
            _questProvider.OnQuestStarted += CollectQuestInfo;
            _questProvider.OnQuestCompleted += OnQuestCompleted;
        }

        private void OnDestroy()
        {
            _buyModuleButton.onClick.RemoveListener(OnBuyUpgradeModuleClicked);
            _openInventoryButton.onClick.AddListener(ToggleOpenAnimation);
            InventorySlot.OnModuleChanged -= UpdatePlacedSlotsCountText;
            _questProvider.OnQuestStarted -= CollectQuestInfo;
            _questProvider.OnQuestCompleted -= OnQuestCompleted;
        }
    }
}