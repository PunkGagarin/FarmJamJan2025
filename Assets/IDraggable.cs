using UnityEngine;

public interface IDraggable
{
    public Sprite Icon { get; }
    public GameObject GameObject { get; }

    void SetIcon(Sprite icon);
    void SetActive(bool isActive);
}