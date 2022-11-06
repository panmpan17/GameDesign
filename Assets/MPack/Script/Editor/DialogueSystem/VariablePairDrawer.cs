using UnityEditor;
using UnityEngine;

namespace MPack
{
    [CustomPropertyDrawer(typeof(DialogueGraph.VariablePair))]
    public class VariablePairDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = 18;
            position.y += 1;

            SerializedProperty keyNameProperty = property.FindPropertyRelative("keyName");
            keyNameProperty.stringValue = EditorGUI.TextField(position, "Key Name", keyNameProperty.stringValue);

            position.y += 20;
            SerializedProperty keyTypeProperty = property.FindPropertyRelative("keyType");
            DialogueGraph.VaribleType enumValue = (DialogueGraph.VaribleType)keyTypeProperty.enumValueIndex;

            enumValue = (DialogueGraph.VaribleType)EditorGUI.EnumPopup(position, "Key Type", enumValue);

            keyTypeProperty.enumValueIndex = (int)enumValue;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 40;
        }
    }
}