using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Farm.Gameplay.Quests.Editor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumTooltipAttribute : PropertyAttribute
    {
        public Dictionary<Enum, string> Tooltips = new Dictionary<Enum, string>();

        public EnumTooltipAttribute(Type enumType)
        {
            foreach (var value in Enum.GetValues(enumType))
            {
                var field = enumType.GetField(value.ToString());
                var tooltip = field.GetCustomAttribute<TooltipAttribute>()?.tooltip ?? "";
                Tooltips[(Enum)value] = tooltip;
            }
        }
    }
}
