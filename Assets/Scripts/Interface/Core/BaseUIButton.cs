using Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BaseUIButton : MonoBehaviour
{
    private Button _button;
    [Inject] private SoundManager _soundManager;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        _soundManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
    }

    private void OnDestroy()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }
}