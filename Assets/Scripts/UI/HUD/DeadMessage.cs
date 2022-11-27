using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
// using UnityEngine.UI;

public class DeadMessage : MonoBehaviour
{
    public static DeadMessage ins;

    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Timer fadeInTimer;
    [SerializeField]
    private Timer fadeOutTimer;
    [SerializeField]
    private GameObject text;
    // [SerializeField]
    // private 

    void Awake()
    {
        ins = this;
        canvasGroup.alpha = 0;
    }

    public void StartFadeIn()
    {
        StartCoroutine(C_FadeIn());
    }

    IEnumerator C_FadeIn()
    {
        text.SetActive(true);

        canvasGroup.alpha = 0;
        fadeInTimer.Reset();
        while (!fadeInTimer.UpdateEnd)
        {
            yield return null;
            canvasGroup.alpha = fadeInTimer.Progress;
        }

        canvasGroup.alpha = 1;
    }


    public void StartFadeOut()
    {
        StartCoroutine(C_FadeOut());
    }

    IEnumerator C_FadeOut()
    {
        text.SetActive(false);

        canvasGroup.alpha = 1;
        fadeOutTimer.Reset();
        while (!fadeOutTimer.UpdateEnd)
        {
            yield return null;
            canvasGroup.alpha = 1 - fadeOutTimer.Progress;
        }

        canvasGroup.alpha = 0;
    }
}
