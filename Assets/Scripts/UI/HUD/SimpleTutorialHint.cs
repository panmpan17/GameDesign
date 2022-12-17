using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;


public class SimpleTutorialHint : MonoBehaviour
{
    public static SimpleTutorialHint _ins;
    public static SimpleTutorialHint ins {
        get {
            if (!_ins)
                _ins = GameObject.Find("HUD").GetComponent<SimpleTutorialHint>();
            return _ins;
        }
    }

    [SerializeField]
    private GameObject hintGameObject;
    [SerializeField]
    private LanguageText hintText;

    [SerializeField]
    private Image focusFrame;

    private TutorialHint _currentHint;

    void Awake()
    {
        if (_currentHint)
            return;

        hintGameObject.SetActive(false);
    }

    public void ShowHint(TutorialHint hint)
    {
        _currentHint = hint;

        hintText.ChangeId(hint.ContentLanguageID);
        hintGameObject.SetActive(true);

        if (hint.RemainTime.Enable)
        {
            StartCoroutine(DelayExecute(hint.RemainTime.Value, CloseHint));
        }
    }

    IEnumerator DelayExecute(float duration, System.Action complete)
    {
        yield return new WaitForSeconds(duration);
        complete.Invoke();
    }

    void CloseHint()
    {
        _currentHint = null;
        hintGameObject.SetActive(false);
    }

    public void CloseHint(TutorialHint hint)
    {
        if (_currentHint != hint)
            return;

        _currentHint = null;
        hintGameObject.SetActive(false);
    }

    public void FocusAtWorldPosition(Vector3 position)
    {
        Camera camera = Camera.main;
        Vector3 viewportPosition = camera.WorldToViewportPoint(position);
        focusFrame.rectTransform.anchoredPosition = focusFrame.canvas.ViewportToCanvasAnchoredPoint(viewportPosition);
        focusFrame.enabled = true;
    }

    public void CloseFocus()
    {
        focusFrame.enabled = false;
    }
}
