using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Farm
{
    public class CapsuleSlotProvider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ISlot, IBeginDragHandler,
        IDragHandler, IEndDragHandler
    {
        public event Action<IDraggable> OnDragStart;
        public event Action<IDraggable> OnClick;
        public event Action<CapsuleSlotProvider> OnModuleChanged;

        [SerializeField] private Image _icon;

        public bool IsOwn { get; set; }
        public Sprite Icon => _icon.sprite;
        public bool CanPlaceItem => IsOwn && _item == null;

        private IDraggable _item;

        public void SetItem(IDraggable item)
        {
            _item = item;
            _icon.sprite = item.Icon;
            _icon.enabled = true;
            OnModuleChanged?.Invoke(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_item == null) return;
            OnDragStart?.Invoke(_item);
            _item = null;
            _icon.enabled = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_item == null) return;
            OnClick?.Invoke(_item);
            _item = null;
            _icon.enabled = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }
    }
}