using System;
using Audio;
using Farm.Interface;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Farm.Gameplay.Capsules
{
    public class CapsuleSlot : MonoBehaviour, IPointerDownHandler, ISlot
    {
        public static event Action<UpgradeModule> OnSlotClick;
        public static event Action<CapsuleSlot> OnAnyModuleChanged;
        public static event Action<CapsuleSlot> OnModuleAddingError;

        [Inject] private InventoryUI _inventoryUI;
        [Inject] private SoundManager _soundManager;

        [SerializeField] private GameObject _icon;

        private UpgradeModule _upgradeModule;

        public bool IsOwn { get; set; }
        public bool IsCapsuleInGrowthProcess { get; set; }
        public bool CanPlaceItem => IsOwn && _upgradeModule == null && !IsCapsuleInGrowthProcess;
        public UpgradeModule UpgradeModule => _upgradeModule;

        public void SetItem(UpgradeModule item)
        {
            _soundManager.PlaySoundByType(GameAudioType.ModuleAddedAction, 0);
            _upgradeModule = item;
            _icon.SetActive(true);
            OnAnyModuleChanged?.Invoke(this);
        }

        public void ShowNotAbleToPlace()
        {
            _soundManager.PlaySoundByType(GameAudioType.ActionError, 0);
            OnModuleAddingError?.Invoke(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RemoveModule();
        }

        public void RemoveModule()
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

            _soundManager.PlaySoundByType(GameAudioType.ModuleRemovedFromCapsuleAction, 0);
            OnSlotClick?.Invoke(_upgradeModule);
            _upgradeModule = null;
            _icon.SetActive(false);
            OnAnyModuleChanged?.Invoke(this);
        }
    }
}