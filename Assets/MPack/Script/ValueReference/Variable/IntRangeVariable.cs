using UnityEngine;


namespace MPack
{
    [CreateAssetMenu(menuName="MPack/Variable/Int Range", order=0)]
    public class IntRangeVariable : ScriptableObject
    {
        public int Min;
        public int Max;
    }
}