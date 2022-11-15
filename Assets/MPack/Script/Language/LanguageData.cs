using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace MPack {
    [CreateAssetMenu(menuName="MPack/Language Data")]
    public class LanguageData : ScriptableObject {
        public int ID;
        public IDTextPair[] Texts;

        [System.Serializable]
        public struct IDTextPair {
            public int ID;
            public string Text;
        }

    #if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(IDTextPair))]
        public class _PropertyDrawer : PropertyDrawer {
            private const float Height = 18, LineHeight = 15;

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                SerializedProperty text = property.FindPropertyRelative("Text");
                int lineCount = text.stringValue.Split(new string[] { "\n" }, System.StringSplitOptions.None).Length - 1;
                return Height + LineHeight * (lineCount + 1);
            }

            public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
            {
                float originalHeight = rect.height;
                rect.width /= 2;
                rect.height = Height;
                EditorGUI.PropertyField(rect, property.FindPropertyRelative("ID"), GUIContent.none);
                rect.width *= 2;
                rect.y += Height;
                rect.height = originalHeight - Height;
                SerializedProperty text = property.FindPropertyRelative("Text");
                text.stringValue = EditorGUI.TextArea(rect, text.stringValue);
            }
        }

        [CustomEditor(typeof(LanguageData))]
        public class _Editor : Editor {
            ReorderableList Texts;

            private void OnEnable()
            {
                Texts = new ReorderableList(serializedObject, serializedObject.FindProperty("Texts"));
                Texts.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Texts");
                Texts.elementHeightCallback = (index) => {
                    return EditorGUI.GetPropertyHeight(Texts.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
                };
                Texts.drawElementCallback = (rect, index, _f, _s) => {
                    EditorGUI.PropertyField(rect, Texts.serializedProperty.GetArrayElementAtIndex(index));
                };
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ID"));
                GUILayout.Space(5);
                Texts.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }
        }
    #endif
    }
}