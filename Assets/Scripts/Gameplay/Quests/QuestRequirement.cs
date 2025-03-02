using System;
using UnityEngine;

namespace Farm.Gameplay.Quests
{
    [Serializable]
    public class QuestRequirement
    {
        public RequirementType RequirementType;
        [Min(1)] public int RequiredAmount;
        [Min(1)] public int RequiredExtraAmount;
        [Tooltip("Описание состояния квеста, которое будет отображаться в интерфейсе. Запись [RequiredExtraAmount] (со скобками) будет заменен на значение в поле RequiredExtraAmount")] public string QuestStateDescription;
    }
}
