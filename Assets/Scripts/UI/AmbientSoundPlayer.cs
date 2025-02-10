using Audio;
using UnityEngine;
using Zenject;

public class AmbientSoundPlayer : MonoBehaviour //todo
{
    private float _minInterval = 15f; 
    private float _maxInterval = 60f;
    
    [Inject] private SoundManager _sfxManager;
    void Start()
    {
        ScheduleNextSound();
    }

   private void ScheduleNextSound()
    {
        float delay = Random.Range(_minInterval, _maxInterval);
        Invoke("PlaySoundAndReschedule", delay);
    }

    private void PlaySoundAndReschedule()
    {
        _sfxManager.PlayRandomSoundByType(GameAudioType.AmbientSound, Vector3.one);
        ScheduleNextSound();
    }
}