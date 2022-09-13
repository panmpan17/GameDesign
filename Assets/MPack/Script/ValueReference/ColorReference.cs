using UnityEngine;

namespace MPack
{
    [System.Serializable]
    public class ColorReference
    {
        public Color Constant;
        public ColorVariable Variable;

        public bool UseVariable;

        public Color Value
        {
            get
            {
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