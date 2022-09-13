using UnityEngine;


namespace MPack
{
    [System.Serializable]
    public struct RangeReference
    {
        public float ConstantMin;
        public float ConstantMax;

        public RangeVariable Variable;

        public bool UseVariable;

        public float Min
        {
            get
            {
#if UNITY_EDITOR
                if (UseVariable)
                    return Variable ? Variable.Min : throw new System.NullReferenceException("Use Varible but varible not exist");
                return ConstantMin;
#else
                return UseVariable ? Variable.Min : ConstantMin;
#endif
            }
        }

        public float Max
        {
            get
            {
#if UNITY_EDITOR
                if (UseVariable)
                    return Variable ? Variable.Max : throw new System.NullReferenceException("Use Varible but varible not exist");
                return ConstantMax;
#else
                return UseVariable ? Variable.Max : ConstantMax;
#endif
            }
        }

        public float PickRandomNumber()
        {
            return UseVariable ? Random.Range(Variable.Min, Variable.Max) : Random.Range(Min, Max);
        }

        public float Clamp(float number)
        {
            return UseVariable ? Mathf.Clamp(number, Variable.Min, Variable.Max) : Mathf.Clamp(number, Min, Max);
        }
    }
}