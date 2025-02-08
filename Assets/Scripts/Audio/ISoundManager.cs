using UnityEngine;

namespace Audio
{
    public interface ISoundManager
    {
        void PlaySoundByType(GameAudioType type, int soundIndex, Vector3 transformPosition);
    }
}