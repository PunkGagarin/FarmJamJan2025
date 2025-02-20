using UnityEngine;

namespace Farm
{
    public class BusketSlot : MonoBehaviour, ISlot
    {
        public bool CanPlaceItem => true;

        public void SetItem(UpgradeModule item)
        {
        }
    }
}