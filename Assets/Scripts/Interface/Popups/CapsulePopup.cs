using Farm.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Interface.Popups
{
    public class CapsulePopup : Popup
    {
        [SerializeField] private Image _embryoView;
        [SerializeField] private Button _createEmbryoButton;
        [SerializeField] private Button _closeButton;
        private Capsule _capsule;

        public void Initialize(Capsule capsule)
        {
            _capsule = capsule;

            _createEmbryoButton.interactable = _capsule.Embryo == null;
            
            _createEmbryoButton.onClick.AddListener(StartEmbryoProcess);
            _closeButton.onClick.AddListener(Close);
            _capsule.OnEmbryoStateChanged += UpdateEmbryoView;
            
            UpdateEmbryoView();
        }

        public override void Close()
        {
            _createEmbryoButton.onClick.RemoveListener(StartEmbryoProcess);
            _closeButton.onClick.RemoveListener(Close);
            _capsule.OnEmbryoStateChanged -= UpdateEmbryoView;
            _createEmbryoButton.interactable = false;

            base.Close();
        }

        private void StartEmbryoProcess()
        {
            if (_capsule.Embryo != null)
                return;

            _capsule.StartEmbryoProcess();
            _embryoView.sprite = _capsule.Embryo.Image;
        }

        private void UpdateEmbryoView()
        {
            _embryoView.gameObject.SetActive(_capsule.Embryo != null);
            _embryoView.sprite = _capsule.Embryo == null ? null : _capsule.Embryo.Image;
        }
    }
}
