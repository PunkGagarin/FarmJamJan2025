using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Farm
{
    public class InventorySlot : MonoBehaviour, IPointerDownHandler, IDraggable
    {
        [SerializeField] private Image _image;
        public GameObject GameObject => gameObject;
        
        public void SetIcon(Sprite icon)
        {
            /*_image.sprite = icon;*/
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public Sprite Icon => _image.sprite;

        public event Action<IDraggable> OnClick;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClick?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}