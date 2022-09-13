using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MPack {
    [CustomPropertyDrawer(typeof(Timer))]
    public class TimerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.width = position.width / 2 - 5;
            EditorGUI.LabelField(position, label);
            position.x += position.width + 2;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("TargetTime"), GUIContent.none);
        }
    }
}