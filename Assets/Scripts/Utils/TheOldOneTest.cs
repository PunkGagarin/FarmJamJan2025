using Farm.Gameplay;
using UnityEngine;

namespace Farm.Utils
{
    public class TheOldOneTest : MonoBehaviour
    {
        [SerializeField] private TheOldOneDefinition _theOldOneDefinition;
        [SerializeField] private TheOldOne _theOldOne;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q)) 
                InitializeTheOldOne();

            if (Input.GetKeyDown(KeyCode.E))
                _theOldOne.Feed(100);
        }

        private void InitializeTheOldOne() => 
            _theOldOne.Initialize(_theOldOneDefinition);
    }
}
