using System.Collections.Generic;
using Farm.Audio;
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

        public void Open(string questDescription, List<QuestInfo> questInfos)
        {
            _questDescription.text = questDescription;
            _questRequirements.text = GetQuestRequirementsText(questInfos);
            
            Open(true);
        }
        
        private string GetQuestRequirementsText(List<QuestInfo> questInfos)
        {
            string text = "";
            foreach (QuestInfo info in questInfos)
            {
                string textToAdd = info.QuestStateDescription;
                textToAdd = textToAdd.Replace("[RequiredExtraAmount]", info.RequiredExtraAmount.ToString());
                textToAdd += info.IsCompleted ? $" {COMPLETED_INFO}" : $"\r\n{info.CurrentAmount} / {info.RequiredAmount}";
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
