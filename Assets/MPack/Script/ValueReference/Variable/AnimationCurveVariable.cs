using UnityEngine;


namespace MPack
{
    [CreateAssetMenu(menuName="MPack/Variable/Animation Curve", order=0)]
    public class AnimationCurveVariable : ScriptableObject
    {
        public AnimationCurve Value;
    }
}