using UnityEngine;

namespace Audio
{
    public class MusicManager : BaseAudioManager
    {
        private const string PLAYER_PREFS_NAME = "MusicVolume";
        private const float DEFAULT_VOLUME = .5f;
        private const float PERCENT_VALUE = 100f;
        
        [SerializeField] private SoundsFactorySO _soundsFactory;

        private bool _isIntroPlaying = false;

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();

            SetPlayerPrefsName();
            Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_NAME, DEFAULT_VOLUME);
            AudioSource.volume = Volume;
        }

        private void Update()
        {
            if (AudioSource.isPlaying || AudioSource.time != 0 || !_isIntroPlaying) return;
            _isIntroPlaying = false;
            PlayBgm();
        }

        public void PlayBgmWithIntro()
        {
            _isIntroPlaying = true;
            var soundToPlay = _soundsFactory.GetClipByTypeAndIndex(GameAudioType.GamePlayIntro, 0);
            AudioSource.clip = soundToPlay.value;
            AudioSource.volume = Volume * (soundToPlay.key / PERCENT_VALUE);
            AudioSource.loop = false;
            AudioSource.Play();
        }

        private void PlayBgm()
        {
            PlaySoundByType(GameAudioType.GamePlayBgm, 0);
        }

        public void PlaySoundByType(GameAudioType type, int soundIndex)
        {
            var soundToPlay = _soundsFactory.GetClipByTypeAndIndex(type, soundIndex);
            AudioSource.clip = soundToPlay.value;
            AudioSource.volume = Volume * (soundToPlay.key / PERCENT_VALUE);
            AudioSource.loop = true;
            AudioSource.Play();
        }
        
        protected override void SetPlayerPrefsName()
        {
            PlayerPrefsName = PLAYER_PREFS_NAME;
        }
    }
}