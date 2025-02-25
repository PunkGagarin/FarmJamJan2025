using System.Collections.Generic;
using Farm.Gameplay.Definitions;
using UnityEngine;

namespace Farm.Gameplay.Quests
{
    [CreateAssetMenu(fileName = "Quest Definition", menuName = "Game Resources/Definition/Quest")]
    public class QuestDefinition : Definition
    {
        [SerializeField, Multiline] private string _description;
        [SerializeField] private List<QuestRequirement> _requirements;
        [SerializeField] private int _energyReward;
        [SerializeField] private int _satietyPenalty;

        public string Description => _description;
        public List<QuestRequirement> Requirements => _requirements;
        public int EnergyReward => _energyReward;
        public int SatietyPenalty => _satietyPenalty;
    }
}
