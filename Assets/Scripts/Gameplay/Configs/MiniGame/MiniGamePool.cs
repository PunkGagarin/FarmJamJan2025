using System.Collections.Generic;
using UnityEngine;

namespace Farm.Gameplay.Configs.MiniGame
{
    [CreateAssetMenu(fileName = "MiniGame Pool", menuName = "Game Resources/Configs/MiniGame Pool")]
    public class MiniGamePool : ScriptableObject
    {
        [SerializeField] private List<MiniGameEffect> _effects;
        [SerializeField] private MiniGameEffect _firstEffect;

        public List<MiniGameEffect> Effects => _effects;
        public MiniGameEffect FirstEffect => _firstEffect;
    }
}