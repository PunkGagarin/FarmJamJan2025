using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio
{

    [CreateAssetMenu(menuName = "Gameplay/Audio/SoundsFactorySO", fileName = "SoundsFactorySO")]
    public class SoundsFactorySO : ScriptableObject
    {
        [SerializeField]
        private List<CustomKeyValue<GameAudioType, List<AudioClip>>> _clips;

        public AudioClip GetRandomClipByType(GameAudioType type)
        {
            AudioClip clipToReturn = null;
            var audioList = _clips.FirstOrDefault(el => el.key == type);
            if (audioList != null)
            {
                clipToReturn = audioList.value[Random.Range(0, audioList.value.Count)];
            }
            
            if (clipToReturn == null)
            {
                Debug.LogError("Clip not found: " + type);
            }
            
            return clipToReturn;
        }

        public AudioClip GetClipByTypeAndIndex(GameAudioType type, int index)
        {
            AudioClip clipToReturn = null;
            var audioList = _clips.FirstOrDefault(el => el.key == type);
            
            if (audioList != null)
            {
                clipToReturn = audioList.value[index];
            }
            
            if (clipToReturn == null)
            {
                Debug.LogError("Clip not found: " + type);
            }
            
            return clipToReturn;
        }
    }

}