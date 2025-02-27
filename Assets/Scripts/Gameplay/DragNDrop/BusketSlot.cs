using Audio;
using UnityEngine;
using Zenject;

namespace Farm
{
    public class BusketSlot : MonoBehaviour, ISlot
    {
        [Inject] private SoundManager _soundManager;

        public bool CanPlaceItem => true;

        public void SetItem(UpgradeModule item)
        {
            _soundManager.PlaySoundByType(GameAudioType.ModuleDestroyedAction, 0);
        }
    }
}