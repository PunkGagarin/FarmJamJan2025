using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Farm.Gameplay.DragNDrop
{
    public class InventorySlot : MonoBehaviour, IPointerDownHandler, IDraggable
    {
        public event Action<IDraggable> OnDragStart;
        public static event Action OnModuleChanged;

        [SerializeField] private StatInfo _firstStatInfo;
        [SerializeField] private StatInfo _secondStatInfo;
        [SerializeField] private StatInfo _thirdStatInfo;
        
        public Sprite Icon => null;
        public UpgradeModule UpgradeModule { get; private set; }

        public void Initialize()
        {
            gameObject.SetActive(false);
        }

        public void SetModule(UpgradeModule module)
        {
            UpgradeModule = module;
            for (var i = 0; i < UpgradeModule.Stats.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        _firstStatInfo.SetStatInfo(UpgradeModule.Stats[i]);
                        _firstStatInfo.gameObject.SetActive(true);
                        break;
                    case 1:
                        _secondStatInfo.SetStatInfo(UpgradeModule.Stats[i]);
                        _secondStatInfo.gameObject.SetActive(true);
                        break;
                    case 3:
                        _thirdStatInfo.SetStatInfo(UpgradeModule.Stats[i]);
                        _thirdStatInfo.gameObject.SetActive(true);
                        break;
                }
            }
            OnModuleChanged?.Invoke();
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