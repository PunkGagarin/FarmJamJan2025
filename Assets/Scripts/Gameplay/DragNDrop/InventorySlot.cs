using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Farm
{
    public class InventorySlot : MonoBehaviour, IPointerDownHandler, IDraggable
    {
        public event Action<IDraggable> OnDragStart;
        public static event Action OnModuleChanged;

        [SerializeField] private Image _image;

        public Sprite Icon => _image.sprite;
        public UpgradeModule UpgradeModule { get; private set; }

        public void Initialize()
        {
            gameObject.SetActive(false);
        }

        public void SetModule(UpgradeModule module)
        {
            UpgradeModule = module;
            OnModuleChanged?.Invoke();
            _image.sprite = module.Definition.Icon;
            gameObject.SetActive(true);
        }

        public void DragEnds(bool success)
        {
            gameObject.SetActive(!success);
            if (success)
            {
                UpgradeModule = null;
                OnModuleChanged?.Invoke();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDragStart?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}