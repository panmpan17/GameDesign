using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu(menuName="Game/BGM Clip")]
public class BGMClip : ScriptableObject
{
    public AudioClip Clip;
    [Range(0, 1)]
    public float VolumeLevel;
    public float FadeInTime;
    public AnimationCurveReference FadeInCurve;
    public float FadeOutTime;
    public AnimationCurveReference FadeOutCurve;
}
