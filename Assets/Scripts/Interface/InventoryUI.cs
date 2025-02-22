using System.Linq;
using Audio;
using DG.Tweening;
using Farm.Gameplay.Repositories;
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

        [Inject] private UpgradeModuleRepository _upgradeModuleRepository;
        [Inject] private SoundManager _soundManager;
        [Inject] private InventoryConfig _inventoryConfig;

        private InventorySlot[] _inventorySlots;
        private float _currentModuleCost;
        private int _currentEnergy;
        private bool _isOpen = false;
        private RectTransform _inventorySlotsContentRect;

        public InventorySlot[] InventorySlots => _inventorySlots;

        public int CurrentEnergy
        {
            get => _currentEnergy;
            set
            {
                if (value == _currentEnergy)
                    return;

                _currentEnergy = value;
                _energyAmount.text = _currentEnergy.ToString();
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

            _soundManager.PlaySoundByType(GameAudioType.ModuleAdded, 0);
            BuyUpgradeModule();
        }

        private void BuyUpgradeModule()
        {
            CurrentEnergy -= (int)_currentModuleCost;
            _currentModuleCost += Mathf.Round(_currentModuleCost * _inventoryConfig.ModuleCostMultiplier);
            UpdateBuyModuleButtonText();
            UpgradeModule newModule = _upgradeModuleRepository.GetRandomModule();
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
            float screenWidth = Screen.width;
            var closedX = screenWidth + _inventorySlotsContentRect.rect.width;
            var openedX = closedX - _inventorySlotsContentRect.rect.width;
            _inventoryRectTransform.DOMoveX(endValue: _isOpen ? closedX : openedX, duration: 0.5f);
            _isOpen = !_isOpen;
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

            _inventorySlotsContentRect = _inventorySlotsContent.GetComponent<RectTransform>();
        }

        private void OnDestroy()
        {
            _buyModuleButton.onClick.RemoveListener(OnBuyUpgradeModuleClicked);
            _openInventoryButton.onClick.AddListener(ToggleOpenAnimation);
            InventorySlot.OnModuleChanged -= UpdatePlacedSlotsCountText;
        }
    }
}