using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using MPack;
using TMPro;

public class SubtitleBehaviour : PlayableBehaviour
{
    public int LanguageID;
    private LanguageText _languageText;
    private TextMeshProUGUI _text;


    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (_languageText == null)
        {
            _languageText = (LanguageText) playerData;
            _text = _languageText.GetComponent<TextMeshProUGUI>();
        }

        if (_languageText.ID != LanguageID)
        {
            _languageText.ChangeId(LanguageID);
        }

        _text.color = new Color(1, 1, 1, info.weight);
    }
}
