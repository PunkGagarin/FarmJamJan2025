using System.Collections.Generic;
using UnityEngine;

namespace Farm.Gameplay.Configs
{
    [CreateAssetMenu(fileName = "Capsule Config", menuName = "Game Resources/Configs/Capsule")]
    public class CapsuleConfig : ScriptableObject
    {
        [SerializeField] private int _upgradeCost;
        [SerializeField, Range(0, 100)] private float _baseHumanChance;
        [SerializeField, Range(0, 100)] private float _baseAnimalChance;
        [SerializeField, Range(0, 100)] private float _baseFishChance;
        [SerializeField, Range(0, 100)] private float _baseMutationChance;
        [SerializeField, Range(0, 100)] private float _basePositiveMutationChance;
        [SerializeField, Tooltip("Нужно выставить по списку стоимость покупки капсулы, при цене 0 - капсула будет доступна со старта игры")] private List<int> _capsuleCosts;

        public List<int> CapsuleCosts => _capsuleCosts;
        public int UpgradeCost => _upgradeCost;
        public float BaseHumanChance => _baseHumanChance;
        public float BaseAnimalChance => _baseAnimalChance;
        public float BaseFishChance => _baseFishChance;
        public float BaseMutationChance => _baseMutationChance;
        public float BasePositiveMutationChance => _basePositiveMutationChance;
    }
}
