using System;
using Audio;
using DG.Tweening;
using Farm.Interface;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Farm.Gameplay.Capsules
{
    public class CapsuleSlot : MonoBehaviour, IPointerDownHandler, ISlot
    {
        public static event Action<UpgradeModule> OnSlotClick;
        public static event Action<CapsuleSlot> OnAnyModuleChanged;

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
            _icon.DOColor(new Color(1, 1, 1, 1), 0f);
            OnAnyModuleChanged?.Invoke(this);
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

            _soundManager.PlaySoundByType(GameAudioType.ModuleRemovedFromCapsule, 0);
            OnSlotClick?.Invoke(_upgradeModule);
            _upgradeModule = null;
            _icon.DOColor(new Color(1, 1, 1, 0), 0f);
            OnAnyModuleChanged?.Invoke(this);
        }
    }
}