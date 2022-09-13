using UnityEngine;


namespace MPack
{
    [System.Serializable]
    public struct IntRangeReference
    {
        public int ConstantMin;
        public int ConstantMax;

        public IntRangeVariable Variable;

        public bool UseVariable;

        public int Min
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

        public int Max
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

        public int PickRandomNumber()
        {
            return UseVariable ? Random.Range(Variable.Min, Variable.Max) : Random.Range(Min, Max);
        }

        public int Clamp(int number)
        {
            return UseVariable ? Mathf.Clamp(number, Variable.Min, Variable.Max) : Mathf.Clamp(number, Min, Max);
        }
    }
}