using System.Collections.Generic;
using Farm.Gameplay.Capsules;
using Farm.Interface;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Farm.Gameplay.DragNDrop
{
    public class IconDragger : MonoBehaviour
    {
        [Inject] private InventoryUI _inventoryUI;

        [SerializeField] private Image _icon;
        [SerializeField] private CapsuleManager _capsuleManager;

        [SerializeField] private GraphicRaycaster _uiRaycaster;
        [SerializeField] private EventSystem _eventSystem;

        private IDraggable _draggable;

        private void Start()
        {
            foreach (var slot in _inventoryUI.InventorySlots) slot.OnDragStart += ChangeSprite;
            CapsuleSlot.OnSlotClick += ReturnModuleToInventory;
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
                    PointerEventData pointerData = new PointerEventData(_eventSystem)
                    {
                        position = Input.mousePosition
                    };

                    List<RaycastResult> results = new List<RaycastResult>();
                    _uiRaycaster.Raycast(pointerData, results);

                    foreach (RaycastResult result in results)
                    {
                        if (result.gameObject.TryGetComponent(out ISlot newSlot))
                        {
                            SetItemNewToSlot(newSlot);
                            return;
                        }
                    }
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
            SetActive(true);
        }

        private void SetActive(bool active)
        {
            _icon.enabled = active;
        }

        private void OnDestroy()
        {
            foreach (var slot in _inventoryUI.InventorySlots) slot.OnDragStart -= ChangeSprite;
            CapsuleSlot.OnSlotClick -= ReturnModuleToInventory;
        }
    }
}