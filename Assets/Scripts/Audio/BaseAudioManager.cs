using UnityEngine;

namespace Audio
{
    public abstract class BaseAudioManager : MonoBehaviour
    {
        protected string PlayerPrefsName;

        protected AudioSource AudioSource;

        public float Volume { get; protected set; }

        public virtual void ChangeVolume(float volume, float masterVolume)
        {
            Volume = volume;
            if (AudioSource != null)
            {
                AudioSource.volume = Volume * masterVolume;
            }
        }

        public virtual void SaveVolume(float volume, float masterVolume)
        {
            ChangeVolume(volume, masterVolume);
            SaveCurrentValue();
        }

        public virtual void SaveCurrentValue()
        {
            PlayerPrefs.SetFloat(PlayerPrefsName, Volume);
            PlayerPrefs.Save();
        }

        public virtual void RestorePrevValue(float masterVolume)
        {
            float prevVolume = PlayerPrefs.GetFloat(PlayerPrefsName, Volume);
            SaveVolume(prevVolume, masterVolume);
        }

        protected abstract void SetPlayerPrefsName();
    }
}