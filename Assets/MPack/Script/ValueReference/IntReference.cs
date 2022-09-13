namespace MPack
{
    [System.Serializable]
    public class IntReference
    {
        public int Constant;
        public IntVariable Variable;

        public bool UseVariable;

        public int Value
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