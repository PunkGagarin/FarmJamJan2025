using Farm.Gameplay.Capsules;
using Farm.Gameplay.MiniGame;
using Farm.Interface.Popups;
using Farm.Utils.Timer;
using UnityEngine;
using Zenject;

namespace Farm.Interface.Tutorial
{
    public class MainTutorial : MonoBehaviour
    {
        [SerializeField] private GameObject _leftShadow;
        [SerializeField] private GameObject _middleShadow;
        [SerializeField] private GameObject _rightShadow;

        [SerializeField] private string _tutorialTextExplain;
        [SerializeField] private string _tutorialTextStartEmbryo;
        [SerializeField] private string _tutorialTextGoodSample;
        [SerializeField] private string _tutorialTextHungry;
        [SerializeField] private string _tutorialTextParadoxMachine;
        [SerializeField] private string _tutorialTextRisk;
        [SerializeField] private string _tutorialTextGettingHungrier;
        [SerializeField] private string _tutorialTextInventory;
        [SerializeField] private string _tutorialTextModule;
        
        [SerializeField] private CapsuleManager _capsuleManager;
        [SerializeField] private Gameplay.TheOldOne _theOldOne;
        [SerializeField] private float _timeToWaitUntilHungryTutorialShow;
        [SerializeField] private int _capsulesOwnToShowInventory = 4;
        
        [Inject] private PopupManager _popupManager;
        [Inject] private TimerService _timerService;
        [Inject] private InventoryUI _inventoryUI;

        public void StartTutorial()
        {
            var message = _popupManager.OpenMessagePopup(_tutorialTextExplain);
            message.SetCloseEvent(OpenFirstCapsule);
            _theOldOne.OnPhaseChanged += OldOnePhaseChanged;
            MiniGameVisual.OnTutorialMiniGameOpened += ShowMiniGameTutorial;
        }

        private void OpenFirstCapsule()
        {
            _capsuleManager.TutorialOpenFirstCapsule();
            CapsulePopup.OnTutorialCapsuleOpened += ShowTutorialCapsuleTutorial;
            CapsulePopup.OnTutorialCapsuleClosed += CapsuleTutorialComplete;
        }

        private void ShowTutorialCapsuleTutorial()
        {
            CapsulePopup.OnTutorialCapsuleOpened -= ShowTutorialCapsuleTutorial;
            _popupManager.OpenMessagePopup(_tutorialTextStartEmbryo, 0);
            CapsulePopup.OnTutorialAnimationCompleted += ShowGoodEmbryoTutorial;
        }
        
        private void ShowGoodEmbryoTutorial()
        {
            CapsulePopup.OnTutorialAnimationCompleted -= ShowGoodEmbryoTutorial;
            _popupManager.OpenMessagePopup(_tutorialTextGoodSample, 3);
        }

        private void CapsuleTutorialComplete()
        {
            CapsulePopup.OnTutorialCapsuleClosed -= CapsuleTutorialComplete;
            _capsuleManager.TutorialOpenRemainCapsules();
            _leftShadow.gameObject.SetActive(false);
            _theOldOne.TutorialCompleted();
            Capsule.OnTutorialCapsuleEmbryoReleased += SetupHungryTutorial;
        }
        
        private void SetupHungryTutorial()
        {
            Capsule.OnTutorialCapsuleEmbryoReleased -= SetupHungryTutorial;
            _timerService.AddTimer(_timeToWaitUntilHungryTutorialShow, ShowHungryTutorial);
        }

        private void ShowHungryTutorial()
        {
            _popupManager.OpenMessagePopup(_tutorialTextHungry, 1);
            _inventoryUI.OnEnergyChanged += CheckCapsuleCost;
        }
        
        private void CheckCapsuleCost()
        {
            var capsule = _capsuleManager.NextUnopenedCapsuleCost();
            if (capsule == null)
            {
                Debug.LogError("something went wrong with capsule cost during tutorial!");
                return;
            }

            if (_inventoryUI.CanBuy(capsule.Cost))
            {
                capsule.ShowTutorialCanBuy();
                if (_capsuleManager.CapsulesOwned == _capsulesOwnToShowInventory - 1)
                    Capsule.OnCapsuleBought += CapsuleBoughtTutorial;
            }
        }

        private void CapsuleBoughtTutorial()
        {
            Capsule.OnCapsuleBought -= CapsuleBoughtTutorial;
            _rightShadow.gameObject.SetActive(false);
            _inventoryUI.TutorialToggleOpenAnimation();
            var message = _popupManager.OpenMessagePopup(_tutorialTextInventory, 2);
            message.SetCloseEvent(() => _popupManager.OpenMessagePopup(_tutorialTextModule, 2));
        }
        
        private void OldOnePhaseChanged(int newPhase)
        {
            if (newPhase < 1)
                return;
            
            _theOldOne.OnPhaseChanged -= OldOnePhaseChanged;

            _popupManager.OpenMessagePopup(_tutorialTextGettingHungrier, 4);
            _middleShadow.gameObject.SetActive(false);
        }
        
        private void ShowMiniGameTutorial()
        {
            MiniGameVisual.OnTutorialMiniGameOpened -= ShowMiniGameTutorial;
            
            var message = _popupManager.OpenMessagePopup(_tutorialTextParadoxMachine);
            message.SetCloseEvent(() => _popupManager.OpenMessagePopup(_tutorialTextRisk));
        }
    }
}
