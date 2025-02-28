using System.Collections.Generic;
using Farm.Gameplay.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Farm.Interface.TheOldOne
{
    public class QuestUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _requirement;
        [SerializeField] private Image _image;

        [Inject] private QuestProvider _questProvider;
        //public const string COMPLETED_INFO = "<color=green> </color>";

        private void Awake()
        {
            _questProvider.OnQuestStarted += SetupPanel;
            _questProvider.OnQuestUpdated += UpdateQuest;
        }
        
        private void SetupPanel() => 
            _panel.SetActive(_questProvider.GetQuestRequirements().Count > 0);

        private void UpdateQuest() => 
            _requirement.text = GetQuestRequirementsText(_questProvider.GetQuestRequirements());

        private string GetQuestRequirementsText(List<QuestInfo> questInfos)
        {
            string text = "";
            foreach (QuestInfo info in questInfos)
            {
                string textToAdd = info.GetCurrentQuestDescription();
                textToAdd = textToAdd.Replace("[RequiredTier]", info.RequiredTier.ToString());
                textToAdd += $" {info.CurrentAmount} / {info.RequiredAmount}\n";
                text += textToAdd;
            }
            
            return text;
        }
    }
}
