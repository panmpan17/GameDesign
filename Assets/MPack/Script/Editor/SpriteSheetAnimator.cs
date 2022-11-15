using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MPack {
    [CustomEditor(typeof(SpriteSheetAnimator))]
    public class _Editor : Editor
    {
        SpriteSheetAnimator animator;

        SerializedProperty sameInterval;

        ReorderableList keyPoints;

        private void OnEnable() {
            animator = (SpriteSheetAnimator) target;

            sameInterval = serializedObject.FindProperty("sameInterval");

            keyPoints = new ReorderableList(serializedObject, serializedObject.FindProperty("keyPoints"),
                true, true, true, true);
            keyPoints.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Key Points");
            keyPoints.elementHeightCallback = (index) => sameInterval.boolValue? 20: 40;
            keyPoints.drawElementCallback = (rect, index, _a, _b) => {
                rect.y += 1;
                rect.height = 18;
                
                SerializedProperty property = keyPoints.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("Sprite"), GUIContent.none);

                if (!sameInterval.boolValue) {
                    rect.y += 20;
                    rect.height = 18;
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("Interval"));
                }
            };
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
            EditorGUILayout.PropertyField(sameInterval);

            if (sameInterval.boolValue) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("interval"));
            }

            keyPoints.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}