using Farm.Gameplay;
using UnityEngine;

namespace Farm.Interface.Popups
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameOverPopup _gameOverPopup;
        [SerializeField] private CapsulePopup _capsulePopup;

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
    }
}
