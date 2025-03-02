using System;
using UnityEditor;
using UnityEngine;

namespace Farm.Gameplay.Quests.Editor
{
    [CustomPropertyDrawer(typeof(EnumTooltipAttribute))]
    public class EnumTooltipDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumTooltipAttribute enumTooltip = (EnumTooltipAttribute)attribute;
            Enum enumValue = (Enum)Enum.ToObject(fieldInfo.FieldType, property.enumValueIndex);

            string tooltip = enumTooltip.Tooltips.ContainsKey(enumValue) ? enumTooltip.Tooltips[enumValue] : "";
            GUIContent newLabel = new GUIContent(label.text, tooltip);

            EditorGUI.PropertyField(position, property, newLabel);
        }
    }
}
