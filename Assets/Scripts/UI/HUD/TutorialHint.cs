using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu(menuName="Game/Tutorail Hint")]
public class TutorialHint : ScriptableObject
{
    [LauguageID]
    public int ContentLanguageID;
    public ValueWithEnable<float> RemainTime;
}
