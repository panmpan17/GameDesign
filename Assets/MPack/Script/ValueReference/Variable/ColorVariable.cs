using UnityEngine;


namespace MPack
{
    [CreateAssetMenu(menuName="MPack/Variable/Color", order=0)]
    public class ColorVariable : ScriptableObject
    {
        public Color Value;
    }
}