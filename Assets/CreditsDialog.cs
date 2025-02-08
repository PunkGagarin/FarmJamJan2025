using UI.Core;
using UnityEngine;
using UnityEngine.UI;

public class CreditsDialog : BaseUIObject
{
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClicked);
        Hide();
    }

    private void OnCloseButtonClicked()
    {
        Hide();
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }
}