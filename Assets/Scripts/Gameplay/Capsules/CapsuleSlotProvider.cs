using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Farm.Gameplay.Capsules
{
    public class CapsuleSlotProvider : MonoBehaviour, ISlot, IPointerDownHandler
    {
        [SerializeField] private Image _icon;

        private CapsuleSlot _capsuleSlot;
        public bool CanPlaceItem { get; }

        private void Awake()
        {
            CapsuleSlot.OnAnyModuleChanged += OnAnyModuleChanged;
        }

        private void OnAnyModuleChanged(CapsuleSlot slot)
        {
            if (_capsuleSlot == slot)
            {
                _icon.enabled = _capsuleSlot.UpgradeModule != null;
            }
        }

        public void SetItem(UpgradeModule item)
        {
            _capsuleSlot.SetItem(item);
        }

        public void SetSlot(CapsuleSlot slot)
        {
            _capsuleSlot = slot;
            _icon.enabled = _capsuleSlot.UpgradeModule != null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _capsuleSlot.RemoveModule();
        }

        private void OnDestroy()
        {
            CapsuleSlot.OnAnyModuleChanged -= OnAnyModuleChanged;
        }
    }
}