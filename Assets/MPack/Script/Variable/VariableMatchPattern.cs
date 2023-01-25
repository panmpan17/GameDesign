using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MPack
{
    [CreateAssetMenu(menuName="MPack/Variable Match Pattern")]
    public class VariableMatchPattern : ScriptableObject
    {
        public BooleanCompare[] BooleanCompares;
        public FloatCompare[] FloatCompares;
        public IntCompare[] IntCompares;

        public bool IsFullyMatched(VariableStorage variableStorage)
        {
            foreach (BooleanCompare booleanCompare in BooleanCompares)
            {
                bool value;
                if (!variableStorage.GetBool(booleanCompare.VariableName, out value))
                {
                    if (booleanCompare.AllowUnset)
                        continue;
                    else
                        return false;
                }

                switch (booleanCompare.Condition)
                {
                    case BooleanCondition.IsTrue:
                        if (!value) return false;
                        break;
                    case BooleanCondition.IsFalse:
                        if (value) return false;
                        break;
                }
            }

            return true;
        }


        public enum BooleanCondition { IsTrue, IsFalse }
        public enum NumericCondition { IsEqual, IsNotEqual, IsBigger, IsSmaller, IsBiggerOrEqual, IsSmallerOrEqual }

        [System.Serializable]
        public struct BooleanCompare
        {
            public string VariableName;
            public BooleanCondition Condition;
            public bool AllowUnset;
        }
        
        [System.Serializable]
        public struct FloatCompare
        {
            public string VariableName;
            public NumericCondition Condition;
            public bool AllowUnset;
            public float Target;
        }
        [System.Serializable]
        public struct IntCompare
        {
            public string VariableName;
            public NumericCondition Condition;
            public bool AllowUnset;
            public int Target;
        }
    }
}