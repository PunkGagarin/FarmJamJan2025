using System.Collections.Generic;
using System.Linq;
using Farm.Audio.SO;
using UnityEngine;
using Zenject;

namespace Farm.Audio
{
    public class SoundManager : BaseAudioManager, ISoundManager
    {
        private const string PLAYER_PREFS_NAME = "SoundEffectVolume";
        private const float DEFAULT_VOLUME = .5f;
        private const float PERCENT_VALUE = 100f;

        [SerializeField]
        private SoundsFactorySO _soundsFactory;

        [Inject] private MasterSoundManager _masterSoundManager;

        private List<AudioSource> _audioSources;

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            _audioSources = new List<AudioSource> { AudioSource };
            SetPlayerPrefsName();
            Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_NAME, DEFAULT_VOLUME);
            AudioSource.volume = Volume;
        }

        private CustomKeyValue<int, AudioClip> GetRandomSoundByType(GameAudioType type)
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
            PlaySound(soundToPlay.value, soundToPlay.key);
        }

        public void PlaySoundByType(GameAudioType type, int soundIndex)
        {
            var clip = _soundsFactory.GetClipByTypeAndIndex(type, soundIndex);
            PlaySound(clip.value, clip.key);
        }

        public void PlayRandomSoundByTypeWithRandomChance(GameAudioType type, int soundIndex, bool isRandomPitch)
        {
            var chance = Random.Range(0f, 1f);
            if (chance < .5f) return;
            var clip = _soundsFactory.GetRandomClipByType(type);
            PlaySoundWithPitch(clip.value, clip.key, isRandomPitch ? Random.Range(1, 2) : 1);
        }

        public void PlaySoundWithPitch(AudioClip clip, int clipVolume, int pitch)
        {
            var audioSource = GetAvailableAudioSource();
            audioSource.clip = clip;
            audioSource.volume = Volume * (clipVolume / PERCENT_VALUE);
            audioSource.pitch = pitch;
            audioSource.Play();
        }

        private void PlaySound(AudioClip clip, int clipVolume)
        {
            var audioSource = GetAvailableAudioSource();
            audioSource.clip = clip;
            audioSource.volume = _masterSoundManager.Volume * Volume * (clipVolume / PERCENT_VALUE);
            audioSource.Play();
        }

        protected override void SetPlayerPrefsName()
        {
            PlayerPrefsName = PLAYER_PREFS_NAME;
        }
    }
}