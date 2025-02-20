using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Farm
{
    public class CapsuleSlotProvider : MonoBehaviour, IPointerDownHandler, ISlot
    {
        public event Action<IDraggable> OnClick;
        public event Action<CapsuleSlotProvider> OnModuleChanged;

        [SerializeField] private Image _icon;

        public bool IsOwn { get; set; }
        public bool CanPlaceItem => IsOwn && _item == null;

        private IDraggable _item;

        public void SetItem(IDraggable item)
        {
            _item = item;
            _icon.sprite = item.Icon;
            _icon.enabled = true;
            OnModuleChanged?.Invoke(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_item == null) return;
            OnClick?.Invoke(_item);
            _item = null;
            _icon.enabled = false;
        }
    }
}