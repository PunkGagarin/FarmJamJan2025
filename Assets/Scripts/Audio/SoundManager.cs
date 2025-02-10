using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio
{
    public class SoundManager : BaseAudioManager, ISoundManager
    {
        private const string PLAYER_PREFS_NAME = "SoundEffectVolume";
        private const float DEFAULT_VOLUME = .5f;

        [SerializeField] private SoundsFactorySO _soundsFactory;

        private List<AudioSource> _audioSources;

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            _audioSources = new List<AudioSource> { AudioSource };
            SetPlayerPrefsName();
            Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_NAME, DEFAULT_VOLUME);
            AudioSource.volume = Volume;
        }

        private AudioClip GetRandomSoundByType(GameAudioType type)
        {
            return _soundsFactory.GetRandomClipByType(type);
        }

        private AudioSource GetAvailableAudioSource()
        {
            foreach (AudioSource source in _audioSources.Where(source => !source.isPlaying))
            {
                return source;
            }
            
            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.volume = Volume;
            _audioSources.Add(newAudioSource);
            return newAudioSource;
        }

        public void PlayRandomSoundByType(GameAudioType type)
        {
            var soundToPlay = GetRandomSoundByType(type);
            PlaySound(soundToPlay);
        }

        public void PlaySoundByType(GameAudioType type, int soundIndex)
        {
            var clip = _soundsFactory.GetClipByTypeAndIndex(type, soundIndex);
            PlaySound(clip);
        }

        private void PlaySound(AudioClip clip)
        {
            var audioSource = GetAvailableAudioSource();
            audioSource.clip = clip;
            audioSource.volume = Volume;
            audioSource.Play();
        }

        protected override void SetPlayerPrefsName()
        {
            PlayerPrefsName = PLAYER_PREFS_NAME;
        }
    }
}