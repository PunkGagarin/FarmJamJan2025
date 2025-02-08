using UnityEngine;

namespace Farm.Interface.Popups
{
    public class CapsulePopup : Popup
    {
        [SerializeField] private RectTransform _pointer;
        [SerializeField] private Vector3 _pointerLeftPosition;
        [SerializeField] private Vector3 _pointerRightPosition;

        private Vector3 _rightVector = new Vector3(1, -1, 1);

        public void UpdatePosition(Vector3 showPosition, bool openPopupToTheLeft, Canvas canvas)
        {
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(showPosition);
        
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, canvas.worldCamera, out Vector2 localPoint);
            
            RectTransform.anchoredPosition = openPopupToTheLeft ? localPoint - new Vector2(RectTransform.sizeDelta.x, 0) : localPoint;
            _pointer.anchoredPosition = openPopupToTheLeft ? _pointerRightPosition : _pointerLeftPosition;
            _pointer.localScale = openPopupToTheLeft ? _rightVector : Vector3.one;
        }
    }
}
