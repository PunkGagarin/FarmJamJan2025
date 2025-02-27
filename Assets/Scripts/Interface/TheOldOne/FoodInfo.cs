using Farm.Gameplay.Definitions;
using TMPro;
using UnityEngine;

namespace Farm.Interface.TheOldOne
{
    public class FoodInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text _humanPercentage;
        [SerializeField] private TMP_Text _animalPercentage;
        [SerializeField] private TMP_Text _fishPercentage;


        public void SetFoodInfo(TheOldOneDefinition theOldOneDefinition)
        {
            _humanPercentage.text = theOldOneDefinition.HumanSatietyModifier.ToString("0") + "%";
            _animalPercentage.text = theOldOneDefinition.AnimalSatietyModifier.ToString("0") + "%";
            _fishPercentage.text = theOldOneDefinition.FishSatietyModifier.ToString("0") + "%";
        }
    }
}
