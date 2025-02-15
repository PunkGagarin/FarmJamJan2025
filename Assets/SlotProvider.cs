using Farm.Gameplay;
using UnityEngine;

namespace Farm
{
    public class SlotProvider : MonoBehaviour, IDraggable
    {
        [SerializeField] private Capsule _capsule;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public Sprite Icon => _spriteRenderer.sprite;
        public GameObject GameObject => gameObject;
        
        public void SetIcon(Sprite icon)
        {
            _spriteRenderer.sprite = icon;
        }

        public void SetActive(bool isActive)
        {
            _spriteRenderer.enabled = isActive;
        }
    }
}