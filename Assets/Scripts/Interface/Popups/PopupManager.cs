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
            _gameOverPopup.gameObject.SetActive(true);
            _gameOverPopup.Open(true);
            return _gameOverPopup;
        }

        public CapsulePopup OpenCapsule(Capsule capsule)
        {
            _capsulePopup.gameObject.SetActive(true);

            return _capsulePopup;
        }
    }
}
