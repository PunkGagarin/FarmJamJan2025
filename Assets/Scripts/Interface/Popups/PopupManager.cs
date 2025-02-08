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

        public CapsulePopup OpenCapsule(Vector3 showPosition, bool openPopupToTheLeft)
        {
            _capsulePopup.UpdatePosition(showPosition, openPopupToTheLeft, _canvas);
            _capsulePopup.gameObject.SetActive(true);

            return _capsulePopup;
        }
    }
}
