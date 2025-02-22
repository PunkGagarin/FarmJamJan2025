using System;
using Farm.Interface;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Gameplay.Capsules
{
    public class CapsuleEnergyCost : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _costAmount;
        [Inject] private InventoryUI _inventory;
        private int _cost;

        private bool CanBuy => _inventory.CanBuy(_cost);

        public event Action OnBoughtSuccess;
        public void Initialize(int cost)
        {
            _cost = cost;
            _costAmount.text = _cost.ToString();
            gameObject.SetActive(true);
        }

        private void Awake()
        {
            gameObject.SetActive(false);
            _button.onClick.AddListener(OnBuy);
        }

        [UsedImplicitly]
        public void MouseEnter()
        {
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
            if (CanBuy)
            {
                _inventory.CurrentEnergy -= _cost;
                gameObject.SetActive(false);
                _inventory.ResetColor();
                OnBoughtSuccess?.Invoke();
            }
            else
            {
                _inventory.ShakeCanNotBuy();
            }
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnBuy);
        }
    }
}
