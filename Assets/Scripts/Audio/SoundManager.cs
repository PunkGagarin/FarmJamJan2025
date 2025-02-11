using UnityEngine;

namespace Audio
{
    public class SoundManager : BaseAudioManager, ISoundManager
    {
        private const string PLAYER_PREFS_NAME = "SoundEffectVolume";
        private const float DEFAULT_VOLUME = .5f;

        [SerializeField] private SoundsFactorySO _soundsFactory;

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            SetPlayerPrefsName();
            Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_NAME, DEFAULT_VOLUME);
            AudioSource.volume = Volume;
        }

        private AudioClip GetRandomSoundByType(GameAudioType type)
        {
            return _soundsFactory.GetRandomClipByType(type);
        }

        public void PlayRandomSoundByType(GameAudioType type, Transform transform)
        {
            var soundToPlay = GetRandomSoundByType(type);
            PlaySound(soundToPlay, transform.position);
        }

        public void PlayRandomSoundByType(GameAudioType type, Vector3 position)
        {
            var soundToPlay = GetRandomSoundByType(type);
            PlaySound(soundToPlay, position);
        }

        public void PlaySoundByType(GameAudioType type, int soundIndex, Vector3 position)
        {
            var soundToPlay = _soundsFactory.GetClipByTypeAndIndex(type, soundIndex);
            PlaySound(soundToPlay, position);
        }

        public void PlayOneShotByType(GameAudioType type, int soundIndex)
        {
            var soundToPlay = _soundsFactory.GetClipByTypeAndIndex(type, soundIndex);
            AudioSource.clip = soundToPlay;
            AudioSource.volume = Volume;
            AudioSource.panStereo = 0.0f;
            AudioSource.spatialBlend = 0.0f;
            AudioSource.Play();
        }

        private void PlaySound(AudioClip clip, Vector3 position)
        {
            AudioSource.PlayClipAtPoint(clip, position, Volume);
        }

        protected override void SetPlayerPrefsName()
        {
            PlayerPrefsName = PLAYER_PREFS_NAME;
        }
    }
}