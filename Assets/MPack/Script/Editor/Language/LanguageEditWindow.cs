using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MPack {
    public class LanguageEditWindow : EditorWindow
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Number = "0123456789";
        private const string Symbol = "!\"#$%^'()*+,-./:;<=>?@[]\\^_`{}|~";

        private const float LabelWidth = 50, LanguageWidth = 400;

        static private Color IDLabelBGColor = EditorUtilities.From256Color(75, 75, 75);
        static private Color LineColor = EditorUtilities.From256Color(65, 65, 65);

        private bool addAlphabet = true, addNumber = true, addSymbol = true;

        [MenuItem("Window/Language Editor")]
        [MenuItem("MPack/Language Editor")]
        static private void OpenEditorWindow() {
            GetWindow<LanguageEditWindow>("Language Editor");
        }

        private Vector2 scrollViewPos = Vector3.zero;
        private LanguageData[] m_languages;
        private string[] m_languageNames;
        private int[] m_languageDataIDs;

        private int m_displayLanguage;

        private GUIStyle m_scrollBarStyle;
        private GUIStyle m_headerStyle;

        private void OnEnable() {
            string[] files = AssetDatabase.FindAssets("t:LanguageData");
            m_languages = new LanguageData[files.Length];
            m_languageNames = new string[files.Length];

            List<int> dataIdList = new List<int>();
            for (int i = 0; i < files.Length; i++) {
                string path = AssetDatabase.GUIDToAssetPath(files[i]);
                LanguageData data = AssetDatabase.LoadAssetAtPath<LanguageData>(path);
                m_languages[i] = data;
                m_languageNames[i] = data.name;

                for (int j = 0; j < data.Texts.Length; j++) {
                    if (!dataIdList.Contains(data.Texts[j].ID))
                    {
                        dataIdList.Add(data.Texts[j].ID);
                    }
                }
            }

            dataIdList.Sort();
            m_languageDataIDs = dataIdList.ToArray();

            m_scrollBarStyle = new GUIStyle();
            m_scrollBarStyle.normal.textColor = Color.white;

            m_headerStyle = new GUIStyle();
            m_headerStyle.fontSize = 18;
            m_headerStyle.normal.textColor = Color.white;
        }

        private void ScanTextInEveryLanguageData() {
            List<char> allChars = new List<char>();
            if (addAlphabet) allChars.AddRange(Alphabet.ToCharArray());
            if (addNumber) allChars.AddRange(Number.ToCharArray());
            if (addSymbol) allChars.AddRange(Symbol.ToCharArray());

            for (int i = 0; i < m_languages.Length; i++) {
                for (int j = 0; j < m_languages[i].Texts.Length; j++) {
                    char[] chars = m_languages[i].Texts[j].Text.ToCharArray();
                    foreach (char chr in chars) {
                        if (!allChars.Contains(chr)) allChars.Add(chr);
                    }
                }
            }

            string allCharString = new string(allChars.ToArray());
            Debug.Log(allCharString);
        }

        private void DrawRow(int ID, float width) {
            int[] textIndex = new int[m_languages.Length];
            int lineCount = 0;

            // Find out all the text is link to this id
            for (int i = 0; i < m_languages.Length; i++) {
                bool assigned = false;
                for (int j = 0; j < m_languages[i].Texts.Length; j++) {
                    if (m_languages[i].Texts[j].ID == ID) {
                        assigned = true;
                        textIndex[i] = j;

                        int count = m_languages[i].Texts[j].Text.Split(
                            new string[] { "\n" }, System.StringSplitOptions.None).Length;
                        
                        if (count > lineCount) lineCount = count;
                        break;
                    }
                }

                if (!assigned) {
                    Array.Resize(ref m_languages[i].Texts, m_languages[i].Texts.Length + 1);
                    int last = m_languages[i].Texts.Length - 1;
                    m_languages[i].Texts[last].ID = ID;
                    m_languages[i].Texts[last].Text = "";
                    textIndex[i] = last;
                }
            }

            Rect rowRect = EditorGUILayout.GetControlRect(false, (lineCount * 15) + 10, GUILayout.Width(width));
            // EditorGUILayout.EndHorizontal();

            Rect labelRect = rowRect;
            labelRect.width = LabelWidth;
            labelRect.height -= 3;
            labelRect.y++;

            EditorGUI.DrawRect(labelRect, IDLabelBGColor);
            GUI.Label(labelRect, ID.ToString());

            labelRect.x += LabelWidth + 2;
            labelRect.width = LanguageWidth;

            for (int i = 0; i < m_languages.Length; i++) {
                int power = Mathf.RoundToInt(Mathf.Pow(2, i));

                if (m_displayLanguage == -1 || ((power & m_displayLanguage) == power))
                {
                    if (textIndex[i] != -1) {
                        EditorGUI.BeginChangeCheck();
                        m_languages[i].Texts[textIndex[i]].Text = EditorGUI.TextArea(labelRect, m_languages[i].Texts[textIndex[i]].Text);
                        if (EditorGUI.EndChangeCheck())
                            EditorUtility.SetDirty(m_languages[i]);
                    }
                    labelRect.x += labelRect.width + 2;
                }
            }

            Rect hrRect = rowRect;
            hrRect.y += rowRect.height - 1;
            hrRect.height = 1;

            EditorGUI.DrawRect(hrRect, LineColor);

            // EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField("", GUILayout.Width(rowRect.width));
            // EditorGUILayout.EndHorizontal();
        }

        private void DrawHeaderRow(float width)
        {
            Rect rowRect = EditorGUILayout.GetControlRect(false, 30, GUILayout.Width(width));

            Rect labelRect = rowRect;
            labelRect.x += LabelWidth + (LanguageWidth / 2) - 40;
            labelRect.y++;
            labelRect.width = LanguageWidth;
            labelRect.height -= 3;


            for (int i = 0; i < m_languageNames.Length; i++) {
                int power = Mathf.RoundToInt(Mathf.Pow(2, i));

                if (m_displayLanguage == -1 || ((power & m_displayLanguage) == power)) {
                    EditorGUI.LabelField(labelRect, m_languageNames[i], m_headerStyle);
                    labelRect.x += labelRect.width + 2;
                }
            }

            Rect hrRect = rowRect;
            hrRect.y += rowRect.height - 1;
            hrRect.height = 1;

            EditorGUI.DrawRect(hrRect, LineColor);
        }

        private void OnGUI() {
            // addAlphabet = EditorGUILayout.Toggle("Auto Include alphabet", addAlphabet);
            // addNumber = EditorGUILayout.Toggle("Auto Include number", addNumber);
            // addSymbol = EditorGUILayout.Toggle("Audo Include symbol", addSymbol);
            // if (GUILayout.Button("Scan Text in Every LanguageData")) {
            //     ScanTextInEveryLanguageData();
            // }


            m_displayLanguage = EditorGUILayout.MaskField(
                "Display Language", m_displayLanguage, m_languageNames);

            float width = LabelWidth + 2;
            for (int i = 0; i < m_languages.Length; i++)
            {
                int power = Mathf.RoundToInt(Mathf.Pow(2, i));
                if (m_displayLanguage == -1 || ((power & m_displayLanguage) == power))
                {
                    width += LanguageWidth + 2;
                }
            }

            EditorGUILayout.BeginScrollView(
                new Vector2(scrollViewPos.x, 0),
                false,
                false,
                m_scrollBarStyle,
                m_scrollBarStyle,
                m_scrollBarStyle,
                GUILayout.Height(30));
            DrawHeaderRow(width);
            EditorGUILayout.EndScrollView();

            scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos, false, false);

            for (int i = 0; i < m_languageDataIDs.Length; i++) {
                DrawRow(m_languageDataIDs[i], width);
            }
            EditorGUILayout.EndScrollView();
        }

    }
}