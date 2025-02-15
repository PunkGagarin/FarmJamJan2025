using Farm.Interface;
using UnityEngine;
using Zenject;

namespace Farm
{
    public class IconDragger : MonoBehaviour
    {
        [Inject] private InventoryUI inventoryUI;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private IDraggable _draggable;

        private void Awake()
        {
            inventoryUI.InventorySlots.ForEach(slot => slot.OnClick += ChangeSprite);
        }

        void Update()
        {
            Vector3 mousePosition = Input.mousePosition; // Координаты мыши в пикселях
            mousePosition.z = 10f; // Устанавливаем Z, чтобы проекция была перед камерой

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = worldPosition; // Перемещаем объект к позиции мыши

            if (Input.GetMouseButtonUp(0) && _draggable != null)
            {
                var hit = Physics2D.Raycast(worldPosition, Vector2.zero);
                if (hit.collider != null && hit.collider.TryGetComponent(out IDraggable draggable))
                {
                    SetActive(false);
                    draggable.SetIcon(_draggable.Icon);
                    _draggable.SetActive(false);
                    _draggable = null;
                }
                else
                {
                    SetActive(false);
                    _draggable.SetActive(true);
                    _draggable = null;
                }
                
            }
        }

        private void ChangeSprite(IDraggable draggable)
        {
            _draggable = draggable;
            _spriteRenderer.sprite = _draggable.Icon;
            SetActive(true);
        }

        public void SetActive(bool active)
        {
            _spriteRenderer.enabled = active;
        }

        private void OnDestroy()
        {
            inventoryUI.InventorySlots.ForEach(slot => slot.OnClick -= ChangeSprite);
        }
    }
}