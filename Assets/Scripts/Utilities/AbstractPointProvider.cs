using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPointProvider : MonoBehaviour
{
    public abstract Point[] GetPoints();

    [System.Serializable]
    public struct Point
    {
        public Vector3 Poisition;
        public Vector3 Forawrd;
    }
}
