using UnityEngine;
namespace Farm.Audio
{
    class MasterSoundManager : BaseAudioManager
    {
        private const string PLAYER_PREFS_NAME = "MasterSoundVolume";
        private const float DEFAULT_VOLUME = 1f;

        private void Awake()
        {
            SetPlayerPrefsName();
            Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_NAME, DEFAULT_VOLUME);
        }

        public override void ChangeVolume(float volume, float masterVolume)
        {
            Volume = volume;
        }

    
        protected override void SetPlayerPrefsName()
        {
            PlayerPrefsName = PLAYER_PREFS_NAME;
        }
    }
}