using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


namespace MPack
{
    [CreateAssetMenu(menuName="MPack/Sprite Animation")]
    public class SpriteAnimation : ScriptableObject
    {
        public string AnimatinName;
        public bool IsLoop;
        public ValueWithEnable<float> SameInterval;
        public SpriteSheetAnimator.KeyPoint[] KeyPoints;
    }
}