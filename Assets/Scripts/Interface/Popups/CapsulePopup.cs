using Farm.Gameplay;
using Farm.Gameplay.MiniGame;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Interface.Popups
{
    public class CapsulePopup : Popup
    {
        [SerializeField] private Image _embryoView;
        [SerializeField] private Button _createEmbryoButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private MiniGameVisual _miniGame;
        private Capsule _capsule;

        public void Initialize(Capsule capsule)
        {
            _miniGame.gameObject.SetActive(false);
            _capsule = capsule;

            _createEmbryoButton.interactable = _capsule.Embryo == null;
            
            _createEmbryoButton.onClick.AddListener(ShowMiniGame);
            _closeButton.onClick.AddListener(Close);
            _capsule.OnEmbryoStateChanged += UpdateEmbryoView;
            
            UpdateEmbryoView();
        }

        public override void Close()
        {
            _createEmbryoButton.onClick.RemoveListener(ShowMiniGame);
            _closeButton.onClick.RemoveListener(Close);
            _capsule.OnEmbryoStateChanged -= UpdateEmbryoView;
            _createEmbryoButton.interactable = false;

            base.Close();
        }

        private void ShowMiniGame()
        {
            if (_capsule.Embryo != null)
                return;

            _closeButton.gameObject.SetActive(false);
            _miniGame.gameObject.SetActive(true);
            _miniGame.OnMiniGameEnds += StartEmbryoProcess;
        }
        
        private void StartEmbryoProcess(string miniGameResult)
        {
            _closeButton.gameObject.SetActive(true);
            _miniGame.gameObject.SetActive(false);
            Debug.Log($"Risk proc : {miniGameResult}");
            _miniGame.OnMiniGameEnds -= StartEmbryoProcess;

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
