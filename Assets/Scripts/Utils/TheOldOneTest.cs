using Farm.Gameplay;
using Farm.Interface;
using UnityEngine;

namespace Farm.Utils
{
    public class TheOldOneTest : MonoBehaviour
    {
        [SerializeField] private TheOldOneDefinition _theOldOneDefinition;
        [SerializeField] private TheOldOneUI _theOldOneUI;
        [SerializeField] private TheOldOne _theOldOne;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q)) 
                InitializeTheOldOne();
        }

        private void InitializeTheOldOne()
        {
            _theOldOneUI.Bind(_theOldOne);
            _theOldOne.Initialize(_theOldOneDefinition);
        }
    }
}
