using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGate : MonoBehaviour
{
    [SerializeField]
    private PortalGatePair pairedGatePair;

    void Awake()
    {
        pairedGatePair.Register(this);
    }

    public Vector3 Teleport()
    {
        PortalGate gate = pairedGatePair.GetOtherGate(this);
        return gate.transform.position;
    }

    void OnDestory()
    {
        pairedGatePair.Unregister(this);
    }
}
