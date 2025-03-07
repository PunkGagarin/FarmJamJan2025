using Farm.Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace Farm.Interface.Popups
{
    public class OptionsPopup : Popup
    {
        [SerializeField] private Button _acceptButton;
        [SerializeField] private Button _declineButton;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Slider _masterSlider;

        [Inject] private SoundManager _soundManager;
        [Inject] private MusicManager _musicManager;
        [Inject] private MasterSoundManager _masterSoundManager;

        private void Awake()
        {
            _acceptButton.onClick.AddListener(OnAcceptButtonClicked);
            _declineButton.onClick.AddListener(OnDeclineButtonClicked);

            _masterSlider.onValueChanged.AddListener(OnMasterSliderValueChanged);
            _musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
            _sfxSlider.onValueChanged.AddListener(OnSoundSliderValueChanged);
        }

        private void Start() => UpdateSliders();

        private void OnAcceptButtonClicked()
        {
            _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);

            _soundManager.SaveCurrentValue();
            _musicManager.SaveCurrentValue();
            _masterSoundManager.SaveCurrentValue();
            Close();
        }

        private void OnDeclineButtonClicked()
        {
            _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);

            _masterSoundManager.RestorePrevValue(_masterSoundManager.Volume);
            _musicManager.RestorePrevValue(_masterSoundManager.Volume);
            _soundManager.RestorePrevValue(_masterSoundManager.Volume);
            UpdateSliders();
            Close();
        }

        private void UpdateSliders()
        {
            _masterSlider.value = _masterSoundManager.Volume;
            _musicSlider.value = _musicManager.Volume;
            _sfxSlider.value = _soundManager.Volume;
        }

        private void OnMasterSliderValueChanged(float masterValue)
        {
            _masterSoundManager.ChangeVolume(masterValue, masterValue);
            OnMusicSliderValueChanged(_musicManager.Volume);
            OnSoundSliderValueChanged(_soundManager.Volume);
        }

        private void OnMusicSliderValueChanged(float value)
        {
            _musicManager.ChangeVolume(value, _masterSoundManager.Volume);
        }

        private void OnSoundSliderValueChanged(float value)
        {
            _soundManager.ChangeVolume(value, _masterSoundManager.Volume);
        }

        private void OnDestroy()
        {
            _acceptButton.onClick.AddListener(OnAcceptButtonClicked);
            _declineButton.onClick.AddListener(OnDeclineButtonClicked);

            _masterSlider.onValueChanged.AddListener(OnMasterSliderValueChanged);
            _musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
            _sfxSlider.onValueChanged.AddListener(OnSoundSliderValueChanged);
        }
    }
}