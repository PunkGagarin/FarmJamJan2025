using UnityEngine;

namespace Audio
{
    public class MusicManager : BaseAudioManager
    {
        private const string PLAYER_PREFS_NAME = "MusicVolume";
        private const float DEFAULT_VOLUME = .5f;

        [SerializeField] private SoundsFactorySO soundsFactory;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            SetPlayerPrefsName();
            Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_NAME, DEFAULT_VOLUME);
            SetAudioSource(audioSource);
            audioSource.volume = Volume;
        }

        public void PlaySoundByType(GameAudioType type, int soundIndex)
        {
            var soundToPlay = soundsFactory.GetClipByTypeAndIndex(type, soundIndex);
            audioSource.clip = soundToPlay;
            audioSource.volume = DEFAULT_VOLUME * Volume;
            audioSource.loop = true;
            audioSource.Play();
        }

        protected override void SetPlayerPrefsName()
        {
            playerPrefsName = PLAYER_PREFS_NAME;
        }
    }
}