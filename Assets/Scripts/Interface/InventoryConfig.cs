using UnityEngine;

namespace Farm.Interface
{
    [CreateAssetMenu(fileName = "Inventory Config", menuName = "Game Resources/Configs/Inventory")]
    public class InventoryConfig : ScriptableObject
    {
        [Header("Энергия")] 
        [SerializeField] private Color _canBuyColor;
        [SerializeField] private Color _canNotBuyColor;
        [SerializeField] private Color _regularColor;
        [SerializeField] private int _startEnergy;
        [Header("Параметры тряски")] 
        [SerializeField] private float _shakeDuration;
        [SerializeField] private float _shakePower;
        [Header("Инвентарь")]
        [SerializeField] private int _maxSlotsCount = 8;
        [SerializeField] private float _baseModuleCost;
        [SerializeField] private float _moduleCostMultiplier;

        public Color CanBuyColor => _canBuyColor;

        public Color CanNotBuyColor => _canNotBuyColor;

        public Color RegularColor => _regularColor;

        public int StartEnergy => _startEnergy;

        public float ShakeDuration => _shakeDuration;

        public float ShakePower => _shakePower;

        public int MaxSlotsCount => _maxSlotsCount;

        public float BaseModuleCost => _baseModuleCost;

        public float ModuleCostMultiplier => _moduleCostMultiplier;
    }
}