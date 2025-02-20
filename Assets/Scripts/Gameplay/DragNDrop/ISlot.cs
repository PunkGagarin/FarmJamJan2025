public interface ISlot
{
    public bool CanPlaceItem { get; }
    void SetItem(UpgradeModule item);
}