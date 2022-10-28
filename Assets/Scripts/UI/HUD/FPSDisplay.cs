using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MPack;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    private SimpleTimer _timer = new SimpleTimer(0.5f);

    void OnEnable()
    {
        text.enabled = true;
    }
    void OnDisable()
    {
        text.enabled = false;
    }

    void Update()
    {
        if (_timer.UnscaleUpdateTimeEnd)
        {
            _timer.Reset();

            int fps = (int)(1f / Time.unscaledDeltaTime);
            text.text = fps.ToString();
        }
    }
}
