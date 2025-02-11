using UnityEditor;
using UnityEngine;

namespace Farm.Gameplay.Definitions.Editor
{
    [CustomPropertyDrawer(typeof(EmbryoChancesDrawer))]
    public class EmbryoChancesDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float totalHeight = EditorGUIUtility.singleLineHeight + 2;
                float fieldWidth = position.width - 10;

                SerializedProperty humanChance = property.FindPropertyRelative("HumanChance");
                SerializedProperty animalChance = property.FindPropertyRelative("AnimalChance");
                SerializedProperty fishChance = property.FindPropertyRelative("FishChance");

                Rect rect1 = new Rect(position.x, position.y + totalHeight, fieldWidth, EditorGUIUtility.singleLineHeight);
                Rect rect2 = new Rect(position.x, position.y + totalHeight * 2, fieldWidth, EditorGUIUtility.singleLineHeight);
                Rect rect3 = new Rect(position.x, position.y + totalHeight * 3, fieldWidth, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(rect1, humanChance, new GUIContent("Шанс создать человека"));
                EditorGUI.PropertyField(rect2, animalChance, new GUIContent("Шанс создать зверя"));
                EditorGUI.PropertyField(rect3, fishChance, new GUIContent("Шанс создать рыбу"));

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 
            property.isExpanded ? (EditorGUIUtility.singleLineHeight + 2) * 4 : EditorGUIUtility.singleLineHeight;

    }
}
