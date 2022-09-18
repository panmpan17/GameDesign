using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName="MPack/Event Reference", order=0)]
public class EventReference : ScriptableObject
{
    [System.NonSerialized]
    private List<EventDispatcher> eventDispatchers = new List<EventDispatcher>();

    public event System.Action InvokeEvents;
    public event System.Action<float> InvokeFloatEvents;
    public event System.Action<int> InvokeIntEvents;

    public void Invoke()
    {
        for (int i = eventDispatchers.Count - 1; i >= 0; i--)
            eventDispatchers[i].DispatchEvent();

        InvokeEvents?.Invoke();
    }

    public void Invoke(float floatValue)
    {
        for (int i = eventDispatchers.Count - 1; i >= 0; i--)
            eventDispatchers[i].DispatchEvent(floatValue);

        InvokeFloatEvents?.Invoke(floatValue);
    }

    public void Invoke(int intValue)
    {
        for (int i = eventDispatchers.Count - 1; i >= 0; i--)
            eventDispatchers[i].DispatchEvent(intValue);

        InvokeIntEvents?.Invoke(intValue);
    }

    public void RegisterEvent(EventDispatcher dispatcher) => eventDispatchers.Add(dispatcher);
    public void UnregisterEvent(EventDispatcher dispatcher) => eventDispatchers.Remove(dispatcher);
}
