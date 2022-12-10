using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MPack
{
    [CustomPropertyDrawer(typeof(LauguageIDAttribute))]
    public class LanguageIDAttributeDrawer : PropertyDrawer
    {
        public static bool s_languageLoaded;
        public static LanguageData[] s_languageDataReferences;

        private bool _found;
        private int[] idIndexInLanguage;
        private int[] heights;

        private int _cachedID;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!s_languageLoaded)
            {
                s_languageLoaded = true;
                LoadLanguageScriptables();
            }

            // property.serializedObject.Update();
            int idValue = property.intValue;
            int totalHeight = 0;

            if (!_found)
            {
                _found = true;
                idIndexInLanguage = new int[s_languageDataReferences.Length];
                heights = new int[s_languageDataReferences.Length];

                for (int i = 0; i < s_languageDataReferences.Length; i++)
                {
                    LanguageData languageData = s_languageDataReferences[i];

                    int index = Array.FindIndex(languageData.Texts, (pair) => {
                        if (pair.ID == idValue)
                            return true;
                        return false;
                    });

                    idIndexInLanguage[i] = index;
                    
                    int height = 20;

                    if (index != -1)
                    {
                        int lineCount = languageData.Texts[index].Text.Split(
                            new string[] { "\n" }, System.StringSplitOptions.None).Length;

                        height += (lineCount - 1) * 20;
                    }
                    heights[i] = height;

                    totalHeight += height;
                }
            }
            else
            {
                for (int i = 0; i < heights.Length; i++)
                    totalHeight += heights[i];
            }

            return 20 + totalHeight;
        }

        void Test(int idValue)
        {
            if (_cachedID == idValue)
                return;
            _cachedID = idValue;

            idIndexInLanguage = new int[s_languageDataReferences.Length];

            for (int i = 0; i < s_languageDataReferences.Length; i++)
            {
                LanguageData languageData = s_languageDataReferences[i];
                idIndexInLanguage[i] = Array.FindIndex(languageData.Texts, (pair) =>
                {
                    if (pair.ID == idValue)
                        return true;
                    return false;
                });
            }
        }

        void LoadLanguageScriptables()
        {
            string[] assets = AssetDatabase.FindAssets("t: LanguageData");

            s_languageDataReferences = new LanguageData[assets.Length];

            for (int i = 0; i < assets.Length; i++)
            {
                s_languageDataReferences[i] = AssetDatabase.LoadAssetAtPath<LanguageData>(AssetDatabase.GUIDToAssetPath(assets[i]));
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Test(property.intValue);

            Rect idRect = position;
            idRect.height = 18;
            idRect.y += 1;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(idRect, property, label);
            if (EditorGUI.EndChangeCheck())
            {
                _found = false;
                return;
            }

            float totalWidth = position.width - 10;
            float y = position.y + 20;

            for (int languageIndex = 0; languageIndex < s_languageDataReferences.Length; languageIndex++)
            {
                LanguageData languageData = s_languageDataReferences[languageIndex];

                Rect languageNameRect = position;
                languageNameRect.y = y;
                languageNameRect.x += 10;
                languageNameRect.width = totalWidth * 0.25f;
                languageNameRect.height = 20;

                y += heights[languageIndex];

                Rect valueRect = position;
                valueRect.y = languageNameRect.y + 1;
                valueRect.x = languageNameRect.x + languageNameRect.width;
                valueRect.width = totalWidth * 0.75f;
                valueRect.height = heights[languageIndex] - 1;

                EditorGUI.LabelField(languageNameRect, languageData.name);


                int index = idIndexInLanguage[languageIndex];
                if (index == -1)
                {
                    if (GUI.Button(valueRect, "Create"))
                    {
                        CreateIDTextPairInLanguageData(property, languageIndex, languageData);
                    }
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    string newText = EditorGUI.TextArea(valueRect, languageData.Texts[index].Text);
                    if (EditorGUI.EndChangeCheck())
                    {
                        int lineCount = newText.Split(
                            new string[] { "\n" }, System.StringSplitOptions.None).Length;
                        heights[languageIndex] = lineCount * 20;

                        ChangeIDTextPairValue(languageData, property.intValue, index, newText);
                    }
                }
            }
        }

        private void ChangeIDTextPairValue(LanguageData languageData, int textId, int indexInLanguageData, string newText)
        {
            Undo.RecordObject(languageData, "Change language text");
            var languageSerializedObject = new SerializedObject(languageData);
            languageSerializedObject.Update();

            languageData.Texts[indexInLanguageData] = new LanguageData.IDTextPair
            {
                ID = textId,
                Text = newText,
            };

            languageSerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(languageData);
        }

        void CreateIDTextPairInLanguageData(SerializedProperty property, int i, LanguageData languageData)
        {
            Undo.RecordObject(languageData, "Create language text");
            var languageSerializedObject = new SerializedObject(languageData);
            languageSerializedObject.Update();

            LanguageData.IDTextPair[] newTexts = new LanguageData.IDTextPair[languageData.Texts.Length + 1];
            for (int e = 0; e < newTexts.Length - 1; e++)
                newTexts[e] = languageData.Texts[e];

            newTexts[newTexts.Length - 1] = new LanguageData.IDTextPair
            {
                ID = property.intValue,
                Text = "",
            };
            languageData.Texts = newTexts;
            idIndexInLanguage[i] = languageData.Texts.Length - 1;
            languageSerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(languageData);
            AssetDatabase.SaveAssetIfDirty(languageData);
        }
    }
}