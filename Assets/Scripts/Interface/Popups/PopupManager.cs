using System.Collections.Generic;
using Farm.Gameplay.Capsules;
using Farm.Gameplay.Definitions;
using Farm.Gameplay.Quests;
using UnityEngine;

namespace Farm.Interface.Popups
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField] private GameOverPopup _gameOverPopup;
        [SerializeField] private CapsulePopup _capsulePopup;
        [SerializeField] private OptionsPopup _optionsPopup;
        [SerializeField] private CreditsPopup _creditsPopup;
        [SerializeField] private QuestPopup _questPopup;
        [SerializeField] private MiniGamePopup _miniGamePopup;
        [SerializeField] private EndPhasePopup _endPhasePopup;
        [SerializeField] private VictoryPopup _victoryPopup;
        [SerializeField] private MessagePopup _messagePopup;

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

        public OptionsPopup OpenOptions()
        {
            _optionsPopup.Open(true);
            return _optionsPopup;
        }

        public CreditsPopup OpenCredits()
        {
            _creditsPopup.Open(false);
            return _creditsPopup;
        }
        
        public QuestPopup OpenQuest(string questDescription, List<QuestInfo> questInfos)
        {
            _questPopup.Open(questDescription, questInfos);
            
            return _questPopup;
        }

        public MiniGamePopup OpenMiniGame()
        {
            _miniGamePopup.Open();

            return _miniGamePopup;
        }

        public EndPhasePopup OpenEndPhasePopup(TheOldOneDefinition currentOldOne)
        {
            _endPhasePopup.Open(currentOldOne);

            return _endPhasePopup;
        }
        
        public VictoryPopup OpenVictoryPopup()
        {
            _victoryPopup.Open(true);

            return _victoryPopup;
        }

        public MessagePopup OpenMessagePopup(string message)
        {
            _messagePopup.Open(message);

            return _messagePopup;
        }
    }
}
