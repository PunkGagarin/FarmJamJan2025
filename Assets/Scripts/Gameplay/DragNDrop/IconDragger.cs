using Farm.Interface;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm
{
    public class IconDragger : MonoBehaviour
    {
        [Inject] private InventoryUI _inventoryUI;
        [SerializeField] private Image _icon;
        [SerializeField] private CapsuleManager _capsuleManager;

        private IDraggable _draggable;

        private void Start()
        {
            foreach (var slot in _inventoryUI.InventorySlots) slot.OnDragStart += ChangeSprite;
            _capsuleManager.Capsules.ForEach(capsule =>
                capsule.CapsuleSlots.ForEach(slot => slot.OnClick += ReturnModuleToInventory));
            SetActive(false);
        }

        private void ReturnModuleToInventory(UpgradeModule obj)
        {
            if (!_inventoryUI.CanPlaceItem) return;
            _inventoryUI.SetItem(obj);
        }

        void Update()
        {
            MoveIconByMouse();

            if (Input.GetMouseButtonUp(0) && _draggable != null)
            {
                var worldPosition = GetWorldPosition();
                var hit = Physics2D.Raycast(worldPosition, Vector2.zero);
                if (hit.collider != null && hit.collider.TryGetComponent(out ISlot slot) && slot.CanPlaceItem)
                {
                    SetItemNewToSlot(slot);
                }
                else
                {
                    ReturnItem(); 
                }
            }
        }

        private static Vector3 GetWorldPosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f;
            return Camera.main.ScreenToWorldPoint(mousePosition);
        }

        private void MoveIconByMouse()
        {
            transform.position = Input.mousePosition;
        }

        private void SetItemNewToSlot(ISlot slot)
        {
            SetActive(false);
            slot.SetItem(_draggable.UpgradeModule);
            _draggable.DragEnds(true);
            _draggable = null;
        }

        private void ReturnItem()
        {
            SetActive(false);
            _draggable.DragEnds(false);
            _draggable = null;
        }

        private void ChangeSprite(IDraggable draggable)
        {
            _draggable = draggable;
            _icon.sprite = _draggable.Icon;
            SetActive(true);
        }

        public void SetActive(bool active)
        {
            _icon.enabled = active;
        }

        private void OnDestroy()
        {
            foreach (var slot in _inventoryUI.InventorySlots) slot.OnDragStart -= ChangeSprite;
            _capsuleManager.Capsules.ForEach(capsule =>
                capsule.CapsuleSlots.ForEach(slot => slot.OnClick -= ReturnModuleToInventory));
        }
    }
}