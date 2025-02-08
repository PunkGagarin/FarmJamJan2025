using UnityEngine;

namespace UI.Core
{
    public class BaseUIObject : MonoBehaviour
    {
        [SerializeField] protected GameObject content;

        public void Show()
        {
            content.SetActive(true);
        }

        public void Hide()
        {
            content.SetActive(false);
        }

    }
}