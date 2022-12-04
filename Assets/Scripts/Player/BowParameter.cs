using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu(menuName="Game/Bow Parameter")]
public class BowParameter : ScriptableObject
{
    public RangeReference ArrowSpeed;
    public float FirstDrawDuration;
    public float IgnoreGravityTime;

    public float SecondDrawDuration;
    public float SecondDrawExtendIgnoreGravityTime;

    public void CombineParamaters(IEnumerable<BowParameter> parameters)
    {
        if (ArrowSpeed.UseVariable)
        {
            ArrowSpeed.UseVariable = false;
            ArrowSpeed.ConstantMin = 0;
            ArrowSpeed.ConstantMax = 0;
        }

        FirstDrawDuration = 0;
        IgnoreGravityTime = 0;
        SecondDrawDuration = 0;
        SecondDrawExtendIgnoreGravityTime = 0;

        foreach (BowParameter parameter in parameters)
        {
            ArrowSpeed.ConstantMin += parameter.ArrowSpeed.Min;
            ArrowSpeed.ConstantMax += parameter.ArrowSpeed.Max;

            FirstDrawDuration += parameter.FirstDrawDuration;
            IgnoreGravityTime += parameter.IgnoreGravityTime;
            SecondDrawDuration += parameter.SecondDrawDuration;
            SecondDrawExtendIgnoreGravityTime += parameter.SecondDrawExtendIgnoreGravityTime;
        }
    }
}
