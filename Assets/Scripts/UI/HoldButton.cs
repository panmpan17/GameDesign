using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using MPack;


public class HoldButton : Button
{
    [SerializeField]
    private Timer holdTimer;
    [SerializeField]
    private Image progressImage;

    private InputSystemUIInputModule _uiInputModule;

    private bool Pressed => _uiInputModule.submit.action.IsPressed();

    public override void OnPointerClick(PointerEventData eventData) {}
    public override void OnSubmit(BaseEventData eventData) {}

    protected override void Start()
    {
        base.Start();
        _uiInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
        holdTimer.Running = false;
    }

    void Update()
    {
        if (!Pressed)
        {
            if (!holdTimer.Running)
                return;

            holdTimer.Reset();
            progressImage.fillAmount = 0;
            holdTimer.Running = false;
            return;
        }


        if (!holdTimer.Running)
        {
            holdTimer.Running = true;
            return;
        }

        if (holdTimer.UpdateEnd)
        {
            onClick.Invoke();
            enabled = false;
            return;
        }

        progressImage.fillAmount = holdTimer.Progress;
    }
}
