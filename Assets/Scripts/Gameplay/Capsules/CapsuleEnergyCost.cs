using System;
using Audio;
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
        [SerializeField] private Image _upgradeImage;
        [Inject] private InventoryUI _inventory;
        [Inject] private SoundManager _sfxManager;
        private int _cost;

        private bool CanBuy => _inventory.CanBuy(_cost);

        public event Action OnBoughtSuccess;
        
        public void UpdateInfo(int cost, bool isUpdate)
        {
            _upgradeImage.enabled = isUpdate;
            _cost = cost;
            _costAmount.text = _cost.ToString();
        }

        private void Awake() => 
            _button.onClick.AddListener(OnBuy);

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
                _inventory.ResetColor();
                OnBoughtSuccess?.Invoke();
                _sfxManager.PlaySoundByType(GameAudioType.CapsuleBoughtAction, 0);
            }
            else
            {
                _inventory.ShakeCanNotBuy();
                _sfxManager.PlaySoundByType(GameAudioType.ActionError, 0);
            }
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnBuy);
        }
    }
}
