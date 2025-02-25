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

            float height = EditorGUIUtility.singleLineHeight * 2;

            if (requirementType.enumValueIndex == (int)RequirementType.CapsulesWithTierUpgrade)
            {
                height += EditorGUIUtility.singleLineHeight;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var requirementType = property.FindPropertyRelative("RequirementType");
            var requiredAmount = property.FindPropertyRelative("RequiredAmount");
            var requiredTier = property.FindPropertyRelative("RequiredTier");

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), requirementType);

            position.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), requiredAmount);

            if (requirementType.enumValueIndex == (int)RequirementType.CapsulesWithTierUpgrade)
            {
                position.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), requiredTier);
            }
        }
    }
}
