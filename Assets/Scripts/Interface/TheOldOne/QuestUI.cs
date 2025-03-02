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

        [Inject] private QuestService _questService;
        //public const string COMPLETED_INFO = "<color=green> </color>";

        private void Awake()
        {
            _questService.OnQuestStarted += SetupPanel;
            _questService.OnQuestUpdated += UpdateQuest;
            SetupPanel();
        }
        
        private void SetupPanel() => 
            _panel.SetActive(_questService.GetQuestRequirements().Count > 0);

        private void UpdateQuest() => 
            _requirement.text = GetQuestRequirementsText(_questService.GetQuestRequirements());

        private string GetQuestRequirementsText(List<QuestInfo> questInfos)
        {
            string text = "";
            foreach (QuestInfo info in questInfos)
            {
                string textToAdd = info.QuestStateDescription;
                textToAdd = textToAdd.Replace("[RequiredExtraAmount]", info.RequiredExtraAmount.ToString());
                textToAdd += $"\n{info.CurrentAmount} / {info.RequiredAmount}";
                text += textToAdd;
            }
            
            return text;
        }
    }
}
