using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class LanguageAssign : MonoBehaviour
{
    [SerializeField]
    private LanguageData defaultLanguage;

    void Awake()
    {
        LanguageMgr.AssignLanguageData(defaultLanguage);
    }
}
