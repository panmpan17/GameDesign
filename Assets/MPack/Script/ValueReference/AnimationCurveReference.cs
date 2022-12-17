using UnityEngine;

namespace MPack
{
    [System.Serializable]
    public class AnimationCurveReference
    {
        public AnimationCurve Constant;
        public AnimationCurveVariable Variable;

        public bool UseVariable;

        public AnimationCurve Value
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

        public float Evaluate(float time)
        {
#if UNITY_EDITOR
            if (UseVariable)
                return Variable ? Variable.Value.Evaluate(time) : throw new System.NullReferenceException("Use Varible but varible not exist");
            return Constant.Evaluate(time);
#else
            return UseVariable ? Variable.Value.Evaluate(time) : Constant.Evaluate(time);
#endif
        }
    }
}