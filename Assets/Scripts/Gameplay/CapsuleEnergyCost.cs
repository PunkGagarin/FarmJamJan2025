using System;
using Farm.Gameplay.Definitions;
using Farm.Interface;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Farm.Gameplay
{
    public class CapsuleEnergyCost : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _costAmount;
        [Inject] private InventoryUI _inventory;
        private CapsuleDefinition _capsuleDefinition;

        private bool CanBuy => _capsuleDefinition != null && _inventory.CanBuy(_capsuleDefinition.CostToUnlock);

        public event Action OnBoughtSuccess;
        public void Initialize(CapsuleDefinition capsuleDefinition)
        {
            _capsuleDefinition = capsuleDefinition;
            _costAmount.text = capsuleDefinition.CostToUnlock.ToString();
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
                _inventory.CurrentEnergy -= _capsuleDefinition.CostToUnlock;
                gameObject.SetActive(false);
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
