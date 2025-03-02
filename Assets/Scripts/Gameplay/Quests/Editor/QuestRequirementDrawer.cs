using UnityEditor;
using UnityEngine;

namespace Farm.Gameplay.Quests.Editor
{
    [CustomPropertyDrawer(typeof(QuestRequirement))]
    public class QuestRequirementDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var requirementType = property.FindPropertyRelative("RequirementType");

            float height = EditorGUIUtility.singleLineHeight * 3.6f;

            if (requirementType.enumValueIndex is (int)RequirementType.CapsulesWithTierUpgrade or (int)RequirementType.CollectUnitsInSeconds)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var requirementType = property.FindPropertyRelative("RequirementType");
            var requiredAmount = property.FindPropertyRelative("RequiredAmount");
            var requiredExtraAmount = property.FindPropertyRelative("RequiredExtraAmount");
            var questStateDescription = property.FindPropertyRelative("QuestStateDescription");

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), requirementType);

            position.y += EditorGUIUtility.singleLineHeight * 1.2f;

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), requiredAmount);
            
            position.y += EditorGUIUtility.singleLineHeight * 1.2f;

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), questStateDescription);
            
            position.y += EditorGUIUtility.singleLineHeight * .2f;
            
            if (requirementType.enumValueIndex is (int)RequirementType.CapsulesWithTierUpgrade or (int)RequirementType.CollectUnitsInSeconds)
            {
                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), requiredExtraAmount);
            }
        }
    }
}
