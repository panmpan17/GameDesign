using UnityEngine;

namespace MPack
{
    [System.Serializable]
    public class FloatReference
    {
        public float Constant;
        public FloatVariable Variable;

        public bool UseVariable;

        public float Value {
            get {
#if UNITY_EDITOR
                if (UseVariable)
                    return Variable ? Variable.Value : throw new System.NullReferenceException("Use Varible but varible not exist");
                return Constant;
#else
                return UseVariable ? Variable.Value : Constant;
#endif
            }
        }
    }
}