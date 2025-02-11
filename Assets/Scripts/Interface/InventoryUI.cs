using DG.Tweening;
using TMPro;
using UnityEngine;

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
        [Header("Параметры тряски")]
        [SerializeField] private float _shakeDuration;
        [SerializeField] private float _shakePower;
        

        private int _currentEnergy;
        
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
        }
    }
}
