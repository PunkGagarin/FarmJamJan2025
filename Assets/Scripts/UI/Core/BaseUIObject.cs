using UnityEngine;

namespace UI.Core
{
    public class BaseUIObject : MonoBehaviour
    {
        [SerializeField] protected GameObject Content;

        public void Show()
        {
            Content.SetActive(true);
        }

        public void Hide()
        {
            Content.SetActive(false);
        }

    }
}