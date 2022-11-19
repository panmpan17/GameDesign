using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class LanguageAssign : MonoBehaviour
{
    public static LanguageAssign ins;

    [SerializeField]
    private LanguageData[] languages;
    private int _currentIndex;

    void Awake()
    {
        if (ins)
        {
            Destroy(gameObject);
            return;
        }

        ins = this;
        LanguageMgr.AssignLanguageData(languages[0]);
    }

    public void NextLanguage()
    {
        if (++_currentIndex >= languages.Length)
        {
            _currentIndex = 0;
        }

        LanguageMgr.AssignLanguageData(languages[_currentIndex]);
    }

    public void PreviousLanguage()
    {
        if (--_currentIndex < 0)
        {
            _currentIndex = languages.Length - 1;
        }

        LanguageMgr.AssignLanguageData(languages[_currentIndex]);
    }
}
