using Farm.Gameplay;
using UnityEngine;

namespace Farm.Interface.Popups
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField] private GameOverPopup _gameOverPopup;
        [SerializeField] private CapsulePopup _capsulePopup;
        [SerializeField] private OptionsPopup _optionsPopup;
        [SerializeField] private CreditsPopup _creditsPopup;

        public GameOverPopup OpenGameOver()
        {
            _gameOverPopup.Open(true);
            return _gameOverPopup;
        }

        public CapsulePopup OpenCapsule(Capsule capsule)
        {
            _capsulePopup.Initialize(capsule);
            _capsulePopup.Open(false);

            return _capsulePopup;
        }

        public OptionsPopup OpenOptions()
        {
            _optionsPopup.Open(true);
            return _optionsPopup;
        }

        public CreditsPopup OpenCredits()
        {
            _creditsPopup.Open(false);
            return _creditsPopup;
        }
    }
}
