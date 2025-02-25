using System;
using Audio;
using Farm.Interface;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Farm.Gameplay.Capsules
{
    public class CapsuleSlotProvider : MonoBehaviour, IPointerDownHandler, ISlot
    {
        public event Action<UpgradeModule> OnClick;
        public static event Action<CapsuleSlotProvider> OnAnyModuleChanged;

        [Inject] private InventoryUI _inventoryUI;
        [Inject] private SoundManager _soundManager;

        [SerializeField] private Image _icon;
        
        private UpgradeModule _upgradeModule;

        public bool IsOwn { get; set; }
        public bool IsCapsuleInGrowthProcess { get; set; }
        public bool CanPlaceItem => IsOwn && _upgradeModule == null && !IsCapsuleInGrowthProcess;
        public UpgradeModule UpgradeModule => _upgradeModule;

        public void SetItem(UpgradeModule item)
        {
            _soundManager.PlaySoundByType(GameAudioType.ModuleAdded, 0);
            _upgradeModule = item;
            _icon.enabled = true;
            OnAnyModuleChanged?.Invoke(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_upgradeModule == null) return;
            if (IsCapsuleInGrowthProcess)
            {
                _soundManager.PlaySoundByType(GameAudioType.ActionError, 0);
                return;
            }
            if (!_inventoryUI.CanPlaceItem)
            {
                _soundManager.PlaySoundByType(GameAudioType.ActionError, 0);
                _inventoryUI.ShakeNotEnoughPlace();
                return;
            }
            _soundManager.PlaySoundByType(GameAudioType.ModuleRemovedFromCapsule, 0);
            OnClick?.Invoke(_upgradeModule);
            _upgradeModule = null;
            _icon.enabled = false;
            OnAnyModuleChanged?.Invoke(this);
        }
    }
}