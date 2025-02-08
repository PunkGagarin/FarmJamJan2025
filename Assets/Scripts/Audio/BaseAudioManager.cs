using UnityEngine;

namespace Audio
{
    public abstract class BaseAudioManager : MonoBehaviour
    {
        protected string playerPrefsName;

        protected AudioSource audioSource;

        public float Volume { get; protected set; }

        public virtual void ChangeVolume(float _volume, float _masterVolume)
        {
            Volume = _volume;
            if (audioSource != null)
            {
                audioSource.volume = Volume * _masterVolume;
            }
        }

        public virtual void SaveVolume(float _volume, float _masterVolume)
        {
            ChangeVolume(_volume, _masterVolume);
            SaveCurrentValue();
        }

        public virtual void SaveCurrentValue()
        {
            PlayerPrefs.SetFloat(playerPrefsName, Volume);
            PlayerPrefs.Save();
        }

        public virtual void RestorePrevValue(float _masterVolume)
        {
            float prevVolume = PlayerPrefs.GetFloat(playerPrefsName, Volume);
            SaveVolume(prevVolume, _masterVolume);
        }


        protected void SetAudioSource(AudioSource audioSource)
        {
            this.audioSource = audioSource;
        }

        protected abstract void SetPlayerPrefsName();
    }
}