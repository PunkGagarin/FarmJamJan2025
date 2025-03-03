using UnityEngine;
using UnityEngine.UI;
namespace Farm.Interface.Popups
{
    public class CreditsPopup : Popup
    {
        [SerializeField] private Button _closeButton;

        private void Awake()
        {
            _closeButton.onClick.AddListener(Close);
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(Close);
        }
    }
}