using Audio;
using UnityEngine;

class MasterSoundManager : BaseAudioManager
{
    private const string PLAYER_PREFS_NAME = "MasterSoundVolume";
    private const float DEFAULT_VOLUME = 1f;

    private void Awake()
    {
        SetPlayerPrefsName();
        Volume = PlayerPrefs.GetFloat(PLAYER_PREFS_NAME, DEFAULT_VOLUME);
    }

    public override void ChangeVolume(float _volume, float _masterVolume)
    {
        Volume = _volume;
    }

    
    protected override void SetPlayerPrefsName()
    {
        playerPrefsName = PLAYER_PREFS_NAME;
    }
}