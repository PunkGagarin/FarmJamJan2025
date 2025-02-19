using System;
using System.Linq;
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
        [SerializeField] private Transform _energyPanel;
        [SerializeField] private TMP_Text _energyAmount;
        [SerializeField] private Color _canBuyColor;
        [SerializeField] private Color _canNotBuyColor;
        [SerializeField] private Color _regularColor;
        [SerializeField] private int _startEnergy;

        [Header("Параметры тряски")] [SerializeField]
        private float _shakeDuration;

        [SerializeField] private float _shakePower;
        [Header("Инвентарь")] [SerializeField] private int _maxSlotsCount = 8;
        [SerializeField] private InventorySlot _inventorySlotPrefab;
        [SerializeField] private Transform _inventorySlotsContent;

        [SerializeField] private float _baseModuleCost;
        [SerializeField] private float _moduleCostMultiplier;
        [SerializeField] private Button _buyModuleButton;
        [SerializeField] private TMP_Text _placedSlotsCountText;

        [Inject] private UpgradeModuleRepository _upgradeModuleRepository;

        private InventorySlot[] _inventorySlots;
        private float _currentModuleCost;
        private int _currentEnergy;
        
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
                ShakeNotEnoughPlace();
                return;
            }
            if (!CanBuy((int)_currentModuleCost))
            {
                ShakeCanNotBuy();
                return;
            }
                 
            BuyUpgradeModule();
        }

        private void BuyUpgradeModule()
        {
            CurrentEnergy -= (int)_currentModuleCost;
            _currentModuleCost += _currentModuleCost * _moduleCostMultiplier;
            UpgradeModule newModule = _upgradeModuleRepository.GetRandomModule();
            SetItem(newModule);
        }
      
        private void SetItem(UpgradeModule module)
        {
            _inventorySlots.First(slot => slot.UpgradeModule == null).SetModule(module);
            UpdatePlacedSlotsCountText();
        }

        private bool CanPlaceItem => _inventorySlots.Any(slot => slot.UpgradeModule == null);
        
        private void UpdatePlacedSlotsCountText()
        {
            var placedCount = _inventorySlots.Count(slot => slot.UpgradeModule != null);
            _placedSlotsCountText.text = $"{placedCount}/{_maxSlotsCount}"; // todo how to understand when item is removed from inventory
        }

        private void InitializeSlots()
        {
            _inventorySlots = new InventorySlot[_maxSlotsCount];
            for (var i = 0; i < _inventorySlots.Length; i++)
            {
                _inventorySlots[i] = Instantiate(_inventorySlotPrefab, _inventorySlotsContent);
                _inventorySlots[i].Initialize();
            }
            UpdatePlacedSlotsCountText();
        }

        private void ShakeNotEnoughPlace() =>
            _placedSlotsCountText.transform.DOShakePosition(_shakeDuration, Vector3.one * _shakePower);

        public bool CanBuy(int cost) =>
            _currentEnergy >= cost;

        public void ShowCanBuy() =>
            _energyAmount.color = _canBuyColor;

        public void ShowCanNotBuy() =>
            _energyAmount.color = _canNotBuyColor;

        public void ResetColor() =>
            _energyAmount.color = _regularColor;

        public void ShakeCanNotBuy() =>
            _energyPanel.DOShakePosition(_shakeDuration, Vector3.one * _shakePower);

        private void Awake()
        {
            CurrentEnergy = _startEnergy;
            InitializeSlots();
            _currentModuleCost = _baseModuleCost;
            _buyModuleButton.onClick.AddListener(OnBuyUpgradeModuleClicked);
        }

        private void OnDestroy()
        {
            _buyModuleButton.onClick.RemoveListener(OnBuyUpgradeModuleClicked);
        }
    }
}