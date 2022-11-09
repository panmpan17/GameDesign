using System.Collections.Generic;
using UnityEngine;

namespace MPack {
    public static class LanguageMgr
    {
        static private LanguageData languageData;
		// static private List<LanguageText> texts = new List<LanguageText>();

        static public bool DataLoaded {
            get {
                return languageData != null;
            }
        }

		static public void AssignLanguageData(LanguageData newData, bool forceReload=false) {
            if (languageData == newData && !forceReload)
                return;

            languageData = newData;

            LanguageText[] texts = GameObject.FindObjectsOfType<LanguageText>();

            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].Text = GetTextById(texts[i].ID);
            }
		}

        /// <summary>
        /// Return the string accroding to langauge ID
        /// </summary>
        /// <param name="id">LanguageText ID</param>
        /// <returns></returns>
		static public string GetTextById(int id) {
            if (languageData == null) {
            #if UNITY_EDITOR
                Debug.LogErrorFormat("Language data havn't assign");
            #endif
                return "";
            }

            for (int i = 0; i < languageData.Texts.Length; i++) {
                if (languageData.Texts[i].ID == id) return languageData.Texts[i].Text;
                
            }
        #if UNITY_EDITOR
            Debug.LogErrorFormat("Text id '{0}' has no language '{1}'", id, languageData.ID);
        #endif
            return "";
		}
    }
}