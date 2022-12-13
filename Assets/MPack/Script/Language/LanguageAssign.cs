using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class LanguageAssign : MonoBehaviour
{
    public static LanguageAssign ins;
    private static int s_currentIndex = 0;

    [SerializeField]
    private LanguageData[] languages;

    void Awake()
    {
        if (ins)
        {
            Destroy(gameObject);
            return;
        }

        ins = this;
        LanguageMgr.AssignLanguageData(languages[s_currentIndex]);
    }

    public void NextLanguage()
    {
        if (++s_currentIndex >= languages.Length)
        {
            s_currentIndex = 0;
        }

        LanguageMgr.AssignLanguageData(languages[s_currentIndex]);
    }

    public void PreviousLanguage()
    {
        if (--s_currentIndex < 0)
        {
            s_currentIndex = languages.Length - 1;
        }

        LanguageMgr.AssignLanguageData(languages[s_currentIndex]);
    }
}
