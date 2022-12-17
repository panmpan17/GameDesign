using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTutorialHintTrigger : MonoBehaviour
{
    [SerializeField]
    private TutorialHint hint;

    public void Trigger()
    {
        SimpleTutorialHint.ins.ShowHint(hint);
    }

    public void CloseThisHint()
    {
        SimpleTutorialHint.ins.CloseHint(hint);
    }



    public void FocusAtTransform(Transform target)
    {
        SimpleTutorialHint.ins.FocusAtWorldPosition(target.position);
    }
}
