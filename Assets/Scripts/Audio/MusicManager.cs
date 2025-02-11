using UnityEngine;

namespace Audio
{
    public class MusicManager : BaseAudioManager
    {
        private const string PLAYER_PREFS_NAME = "MusicVolume";
        private const float DEFAULT_VOLUME = .5f;

        [SerializeField] private SoundsFactorySO _soundsFactory;

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();

            SetPlayerPrefsName();
            Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_NAME, DEFAULT_VOLUME);
            AudioSource.volume = Volume;
        }

        public void PlaySoundByType(GameAudioType type, int soundIndex)
        {
            var soundToPlay = _soundsFactory.GetClipByTypeAndIndex(type, soundIndex);
            AudioSource.clip = soundToPlay;
            AudioSource.volume = Volume;
            AudioSource.loop = true;
            AudioSource.Play();
        }

        protected override void SetPlayerPrefsName()
        {
            PlayerPrefsName = PLAYER_PREFS_NAME;
        }
    }
}