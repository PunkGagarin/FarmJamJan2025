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
            var value = Mathf.Round(stat.Value);
            var sign = value >= 0 ? "+" : "-";
            var valueString = $"{sign} {Mathf.Abs(value)}%";
            _value.text = valueString;
            _icon.sprite = stat.Icon;
        }
    }
}