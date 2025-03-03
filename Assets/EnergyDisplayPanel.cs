using Farm.Interface;
using TMPro;
using UnityEngine;
using Zenject;

namespace Farm
{
    public class EnergyDisplayPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _energyText;

        [Inject] private InventoryUI _inventoryUI;

        private void Awake() => _inventoryUI.OnEnergyChanged += UpdateEnergy;

        private void UpdateEnergy() => _energyText.text = _inventoryUI.CurrentEnergy.ToString();

        private void OnDestroy() => _inventoryUI.OnEnergyChanged -= UpdateEnergy;
    }
}