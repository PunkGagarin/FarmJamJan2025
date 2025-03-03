using DG.Tweening;
using Farm.Gameplay.DragNDrop;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Farm.Gameplay.Capsules
{
    public class CapsuleSlotProvider : MonoBehaviour, ISlot, IPointerDownHandler
    {
        [SerializeField] private Image _icon;

        private CapsuleSlot _capsuleSlot;

        public bool CanPlaceItem => _capsuleSlot.CanPlaceItem;

        private void Awake() => CapsuleSlot.OnAnyModuleChanged += OnAnyModuleChanged;

        private void OnAnyModuleChanged(CapsuleSlot slot)
        {
            if (_capsuleSlot == slot)
            {
                _icon.DOColor(new Color(1, 1, 1, _capsuleSlot.UpgradeModule != null ? 1 : 0), 0f);
            }
        }

        public void SetItem(UpgradeModule item) => _capsuleSlot.SetItem(item);

        public void ShowNotAbleToPlace() => _capsuleSlot.ShowNotAbleToPlace();

        public void SetSlot(CapsuleSlot slot)
        {
            _capsuleSlot = slot;
            _icon.DOColor(new Color(1, 1, 1, _capsuleSlot.UpgradeModule != null ? 1 : 0), 0f);
        }

        public void OnPointerDown(PointerEventData eventData) => _capsuleSlot.RemoveModule();

        private void OnDestroy() => CapsuleSlot.OnAnyModuleChanged -= OnAnyModuleChanged;
    }
}