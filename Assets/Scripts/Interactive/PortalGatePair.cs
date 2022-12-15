using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName="Game/Portal Gate Pair")]
public class PortalGatePair : ScriptableObject
{
    [System.NonSerialized]
    private PortalGate _portal1, _portal2;

    public void Register(PortalGate gate)
    {
        if (!_portal1)
        {
            _portal1 = gate;
            return;
        }

        if (!_portal2)
        {
            _portal2 = gate;
            return;
        }

#if UNITY_EDITOR
        Debug.LogError("Portal gate excceed apir amount", this);
        Debug.Log("Portal 1", _portal1);
        Debug.Log("Portal 2", _portal2);
        Debug.Log("Extra Portal", gate);
#endif
    }

    public void Unregister(PortalGate gate)
    {
        if (_portal1 == gate)
            _portal1 = null;
        else if (_portal2 == gate)
            _portal2 = null;
    }

    public PortalGate GetOtherGate(PortalGate gate)
    {
        return _portal1 == gate ? _portal2 : _portal1;
    }
}
