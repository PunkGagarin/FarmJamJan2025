using Audio;
using Farm.Utils.Timer;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class AmbientSoundPlayer : MonoBehaviour
{
    private TimerHandle _timerHandle;
    private float _duration = 45f;
    private float _playChance = 0.5f;

    [Inject] private SoundManager _sfxManager;
    [Inject] private TimerService _timerService;

    void Start()
    {
        _timerHandle = _timerService.AddTimer(_duration, TryPlayAmbient, true);
    }

    private void TryPlayAmbient()
    {
        if (Random.value < _playChance)
        {
            _sfxManager.PlayRandomSoundByType(GameAudioType.AmbientSound);
        }
    }

    private void OnDestroy()
    {
        _timerHandle?.FinalizeTimer();
        _timerHandle = null;
    }
}