using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPack
{
    [CreateAssetMenu(menuName = "MPack/Variable/Layer Mask", order = 0)]
    public class SortingLayerVariable : ScriptableObject
    {
        [SortingLayer]
        public int LayerID;
        public int Order;
    }
}
