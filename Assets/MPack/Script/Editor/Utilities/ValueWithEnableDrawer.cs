using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MPack
{
    public class ValueWithEnableDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float originWidth = position.width;
            position.width = 20;

            SerializedProperty isOverride = property.FindPropertyRelative("Enable");
            bool newBool = EditorGUI.Toggle(position, isOverride.boolValue);
            if (newBool != isOverride.boolValue)
            {
                isOverride.boolValue = newBool;
            }

            position.width = originWidth - 20;
            position.x += 20;
            GUI.enabled = newBool;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Value"), label);
        }
    }

    [CustomPropertyDrawer(typeof(ValueWithEnable<int>))]
    public class ValueWithEnableInt : ValueWithEnableDrawer { }

    [CustomPropertyDrawer(typeof(ValueWithEnable<float>))]
    public class ValueWithEnableFloat : ValueWithEnableDrawer { }
}