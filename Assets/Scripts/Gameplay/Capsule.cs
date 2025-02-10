using Farm.Interface.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Gameplay
{
    public class Capsule : MonoBehaviour
    {
        [SerializeField] private Image _timer;
        [SerializeField] private TMP_Text _info;
        
        [Inject] private PopupManager _popupManager;

        private void Awake()
        {
            _timer.gameObject.SetActive(false);
            _info.gameObject.SetActive(false);
        }

        private void OnMouseDown()
        {
            _popupManager.OpenCapsule(this);
        }
    }
}
