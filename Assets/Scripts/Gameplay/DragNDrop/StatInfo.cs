using Farm.Gameplay.Configs.UpgradeModules;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Gameplay.DragNDrop
{
    public class StatInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private Image _icon;

        public void SetStatInfo(UpgradeModuleStat stat)
        {
            _description.text = stat.Description;
            _value.text = Mathf.Round(stat.Value).ToString();
            _icon.sprite = stat.Icon;
        }
    }
}