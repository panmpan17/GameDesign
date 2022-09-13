namespace MPack
{
    [System.Serializable]
    public class BoolReference
    {
        public bool Constant;
        public BoolVariable Variable;

        public bool UseVariable;

        public bool Value
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