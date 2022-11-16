using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPack
{
    [System.Serializable]
    public struct SortingLayerReference
    {
        [SortingLayer]
        public int ConstantLayerID;
        public int ConstantOrder;

        public SortingLayerVariable Variable;

        public bool UseVariable;

        public int LayerID
        {
            get
            {
#if UNITY_EDITOR
                if (UseVariable)
                    return Variable ? Variable.LayerID : throw new System.NullReferenceException("Use Varible but varible not exist");
                return ConstantLayerID;
#else
                return UseVariable ? Variable.LayerID : Constant;
#endif
            }
        }

        public int Order
        {
            get
            {
#if UNITY_EDITOR
                if (UseVariable)
                    return Variable ? Variable.Order : throw new System.NullReferenceException("Use Varible but varible not exist");
                return ConstantOrder;
#else
                return UseVariable ? Variable.Order : Constant;
#endif
            }
        }
    }
}
