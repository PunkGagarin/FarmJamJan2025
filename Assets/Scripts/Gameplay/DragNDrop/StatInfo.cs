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
            var value = Mathf.RoundToInt(stat.Value);
            var sign = value >= 0 ? "+" : "-";
            var valueString = $"{sign} {Mathf.Abs(value)}%";
            _value.text = valueString;
            _value.color = value >= 0 ? new Color(0, 0.3f, 0, 1) : new Color(0.3f, 0, 0, 1);
            _icon.sprite = stat.Icon;
        }
    }
}