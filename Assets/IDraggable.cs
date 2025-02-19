using UnityEngine;

public interface IDraggable
{
    public Sprite Icon { get; }
    public UpgradeModule UpgradeModule { get; }
    
    void DragEnds(bool success);
}