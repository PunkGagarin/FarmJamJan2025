using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class MusicManager : BaseAudioManager
    {
        private const string PLAYER_PREFS_NAME = "MusicVolume";
        private const float DEFAULT_VOLUME = .5f;
        private const float PERCENT_VALUE = 100f;

        [SerializeField] private SoundsFactorySO _soundsFactory;

        private bool _isIntroPlaying = false;
        private GameAudioType _currentClipType = GameAudioType.None;
        private GameAudioType _nextClipTypeToPlay = GameAudioType.None;

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();

            SetPlayerPrefsName();
            Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_NAME, DEFAULT_VOLUME);
            AudioSource.volume = Volume;
        }

        private void Update()
        {
            if (AudioSource.isPlaying || AudioSource.time != 0) return;

            if (_isIntroPlaying)
            {
                _isIntroPlaying = false;
                StartBgm();
                return;
            }
            
            var clipToPlay = _nextClipTypeToPlay != GameAudioType.None && _currentClipType != _nextClipTypeToPlay
                ? _nextClipTypeToPlay
                : _currentClipType;
            PlaySoundByType(clipToPlay, 0);
        }

        public void SetNextClipToPlay(GameAudioType audioType) => 
            _nextClipTypeToPlay = audioType;

        public void PlayBgm()
        {
            var currentScene = SceneManager.GetActiveScene().name;
            var introAudioType = currentScene == SceneLoader.Scene.MainMenuScene.ToString()
                ? GameAudioType.MainMenuIntro
                : GameAudioType.GamePlayIntro;

            StartIntro(introAudioType);
        }

        private void StartIntro(GameAudioType introAudioType)
        {
            _isIntroPlaying = true;
            PlaySoundByType(introAudioType, 0);
        }
        
        private void StartBgm()
        {
            var currentScene = SceneManager.GetActiveScene().name;
            var bgmAudioType = currentScene == SceneLoader.Scene.MainMenuScene.ToString()
                ? GameAudioType.MainMenuBgm
                : GameAudioType.GamePlayBgm;

            PlaySoundByType(bgmAudioType, 0);
        }

        private void PlaySoundByType(GameAudioType type, int soundIndex)
        {
            if (type == GameAudioType.None)
                return;
            
            var soundToPlay = _soundsFactory.GetClipByTypeAndIndex(type, soundIndex);
            AudioSource.clip = soundToPlay.value;
            AudioSource.volume = Volume * (soundToPlay.key / PERCENT_VALUE);
            AudioSource.loop = false;
            AudioSource.Play();
            _currentClipType = type;
        }

        protected override void SetPlayerPrefsName()
        {
            PlayerPrefsName = PLAYER_PREFS_NAME;
        }
    }
}