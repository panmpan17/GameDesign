using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventDispatcher : MonoBehaviour
{
    [SerializeField]
    private EventReference eventReference;

    public UnityEvent Event;
    public FloatUnityEvent FloatEvent;
    public IntUnityEvent IntEvent;

    void OnEnable()
    {
        eventReference.RegisterEvent(this);
    }
    void OnDisable()
    {
        eventReference.UnregisterEvent(this);
    }

    public void DispatchEvent() => Event.Invoke();
    public void DispatchEvent(float floatValue) => FloatEvent.Invoke(floatValue);
    public void DispatchEvent(int intValue) => IntEvent.Invoke(intValue);

    [System.Serializable]
    public class FloatUnityEvent : UnityEvent<float> {}

    [System.Serializable]
    public class IntUnityEvent : UnityEvent<int> { }
}
