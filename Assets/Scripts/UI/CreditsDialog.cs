using UI.Core;
using UnityEngine;
using UnityEngine.UI;

public class CreditsDialog : BaseUIObject
{
    [SerializeField] private Button _closeButton;

    private void Awake()
    {
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        Hide();
    }

    private void OnCloseButtonClicked()
    {
        Hide();
    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }
}