using Farm.Gameplay;
using Farm.Gameplay.Definitions;
using UnityEngine;

namespace Farm.Utils
{
    public class TheOldOneTest : MonoBehaviour
    {
        [SerializeField] private TheOldOneDefinition _theOldOneDefinition;
        [SerializeField] private TheOldOne _theOldOne;

        private void Start()
        {
            InitializeTheOldOne();
        }

        private void InitializeTheOldOne() => 
            _theOldOne.Initialize(_theOldOneDefinition);
    }
}
