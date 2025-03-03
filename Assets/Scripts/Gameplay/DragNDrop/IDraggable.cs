using UnityEngine;

namespace Farm.Gameplay.DragNDrop
{
    public interface IDraggable
    {
        public Sprite Icon { get; }
        public UpgradeModule UpgradeModule { get; }
    
        void DragEnds(bool success);
    }
}