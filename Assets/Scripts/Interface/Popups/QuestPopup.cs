using System.Collections.Generic;
using Audio;
using Farm.Gameplay.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Interface.Popups
{
    public class QuestPopup : Popup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_Text _questDescription;
        [SerializeField] private TMP_Text _questRequirements;
        
        [Inject] private SoundManager _sfxManager;
        
        public const string COMPLETED_INFO = "<color=green>Completed</color>";

        public void Open(string questDefinition, List<QuestInfo> questInfos)
        {
            _questDescription.text = questDefinition;
            _questRequirements.text = GetQuestRequirementsText(questInfos);
            
            Open(true);
        }
        
        private string GetQuestRequirementsText(List<QuestInfo> questInfos)
        {
            string text = "";
            foreach (QuestInfo info in questInfos)
            {
                string textToAdd = info.GetCurrentQuestDescription();
                textToAdd = textToAdd.Replace("[RequiredTier]", info.RequiredTier.ToString());
                textToAdd += info.IsCompleted ? $" {COMPLETED_INFO}" : $" {info.CurrentAmount} / {info.RequiredAmount}\n";
                text += textToAdd;
            }
            
            return text;
        }
        
        private void Awake() => 
            _closeButton.onClick.AddListener(CloseClick);

        private void OnDestroy() => 
            _closeButton.onClick.RemoveListener(CloseClick);

        private void CloseClick()
        {
            _sfxManager.PlaySoundByType(GameAudioType.UiButtonClick, 0);
            Close();
        }
    }
}
