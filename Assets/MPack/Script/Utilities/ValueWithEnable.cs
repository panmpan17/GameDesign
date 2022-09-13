using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MPack
{
    [System.Serializable]
    public struct ValueWithEnable<T> where T : struct
    {
        public bool Enable;
        public T Value;

        public ValueWithEnable(T value, bool enable = false)
        {
            Enable = enable;
            Value = value;
        }
    }
}