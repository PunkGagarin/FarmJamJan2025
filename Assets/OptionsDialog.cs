using UI.Core;
using UnityEngine;
using UnityEngine.UI;

public class OptionsDialog : BaseUIObject
{
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider masterSlider;

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
        //todo
        Hide();
    }

    private void OnDeclineButtonClicked()
    {
        //todo
        Hide();
    }

    private void OnMasterSliderValueChanged(float arg0)
    {
        //todo
    }

    private void OnMusicSliderValueChanged(float value)
    {
        //todo
    }

    private void OnSoundSliderValueChanged(float value)
    {
        //todo
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