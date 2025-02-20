using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Farm
{
    public class CapsuleSlotProvider : MonoBehaviour, IPointerDownHandler, ISlot
    {
        public event Action<UpgradeModule> OnClick;
        public event Action<CapsuleSlotProvider> OnModuleChanged;

        [SerializeField] private Image _icon;

        public bool IsOwn { get; set; }
        public bool CanPlaceItem => IsOwn && (_upgradeModule) == null;

        private UpgradeModule _upgradeModule;

        public void SetItem(UpgradeModule item)
        {
            _upgradeModule = item;
            _icon.sprite = item.Icon;
            _icon.enabled = true;
            OnModuleChanged?.Invoke(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_upgradeModule == null) return;
            OnClick?.Invoke(_upgradeModule);
            _upgradeModule = null;
            _icon.enabled = false;
            OnModuleChanged?.Invoke(this);
        }
    }
}