using Audio;
using UnityEngine;
using Zenject;

namespace Farm
{
    public class BusketSlot : MonoBehaviour, ISlot
    {
        private static readonly int Play = Animator.StringToHash("Play");
        [SerializeField] private Animator _trashBinAnimator;

        [Inject] private SoundManager _soundManager;

        public bool CanPlaceItem => true;

        public void SetItem(UpgradeModule item)
        {
            _trashBinAnimator.SetTrigger(Play);
            _soundManager.PlaySoundByType(GameAudioType.ModuleDestroyedAction, 0);
        }
    }
}