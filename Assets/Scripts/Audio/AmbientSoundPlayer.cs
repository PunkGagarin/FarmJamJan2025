using Farm.Audio.SO;
using Farm.Utils.Timer;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Random = UnityEngine.Random;

namespace Farm.Audio
{
    public class AmbientSoundPlayer : MonoBehaviour
    {
        private TimerHandle _timerHandle;
        private float _playChance = 0.5f;

        [SerializeField] private SoundsFactorySO _soundsFactory;
        [Inject] private SoundManager _sfxManager;
        [Inject] private TimerService _timerService;

        void Start()
        {
            _timerHandle = _timerService.AddTimer(_soundsFactory.IntervalBetweenAmbientSfx, TryPlayAmbient, true);
        }

        private void TryPlayAmbient()
        {
            var currentScene = SceneManager.GetActiveScene().name;
            var chance = Random.Range(0f, 1f);
            if (chance < _playChance && currentScene == SceneLoader.SceneLoader.Scene.Gameplay.ToString())
            {
                _sfxManager.PlayRandomSoundByType(GameAudioType.AmbientSounds);
            }
        }

        private void OnDestroy()
        {
            _timerHandle?.FinalizeTimer();
            _timerHandle = null;
        }
    }
}