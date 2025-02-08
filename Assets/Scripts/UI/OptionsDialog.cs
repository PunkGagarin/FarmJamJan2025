using Audio;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class OptionsDialog : BaseUIObject
{
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider masterSlider;

    [Inject] private SoundManager soundManager;
    [Inject] private MusicManager musicManager;
    [Inject] private MasterSoundManager masterSoundManager;

    private void Awake()
    {
        acceptButton.onClick.AddListener(OnAcceptButtonClicked);
        declineButton.onClick.AddListener(OnDeclineButtonClicked);

        masterSlider.onValueChanged.AddListener(OnMasterSliderValueChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        sfxSlider.onValueChanged.AddListener(OnSoundSliderValueChanged);
        Hide();
    }

    private void OnAcceptButtonClicked()
    {
        soundManager.PlaySoundByType(GameAudioType.ButtonCLick, 0, Vector3.zero);

        soundManager.SaveCurrentValue();
        musicManager.SaveCurrentValue();
        masterSoundManager.SaveCurrentValue();
        Hide();
    }

    private void OnDeclineButtonClicked()
    {
        soundManager.PlaySoundByType(GameAudioType.ButtonCLick, 0, Vector3.zero);

        masterSoundManager.RestorePrevValue(masterSoundManager.Volume);
        musicManager.RestorePrevValue(masterSoundManager.Volume);
        soundManager.RestorePrevValue(masterSoundManager.Volume);
        UpdateSliders();
        Hide();
    }

    private void UpdateSliders()
    {
        masterSlider.value = masterSoundManager.Volume;
        musicSlider.value = musicManager.Volume;
        sfxSlider.value = soundManager.Volume;
    }

    private void OnMasterSliderValueChanged(float value)
    {
        masterSoundManager.ChangeVolume(value, value);
        OnMusicSliderValueChanged(musicManager.Volume);
        OnSoundSliderValueChanged(soundManager.Volume);
    }

    private void OnMusicSliderValueChanged(float value)
    {
        musicManager.ChangeVolume(value, masterSoundManager.Volume);
    }

    private void OnSoundSliderValueChanged(float value)
    {
        soundManager.ChangeVolume(value, masterSoundManager.Volume);
    }

    private void OnDestroy()
    {
        acceptButton.onClick.AddListener(OnAcceptButtonClicked);
        declineButton.onClick.AddListener(OnDeclineButtonClicked);

        masterSlider.onValueChanged.AddListener(OnMasterSliderValueChanged);
        musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        sfxSlider.onValueChanged.AddListener(OnSoundSliderValueChanged);
    }
}