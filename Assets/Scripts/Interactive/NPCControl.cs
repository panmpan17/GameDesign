using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[SelectionBase]
public class NPCControl : MonoBehaviour
{
    public const string Tag = "NPC";

    [SerializeField]
    private TransformPointer playerFeetPointer;
    [SerializeField]
    private DialogueGraph dialogueGraph;
    [SerializeField]
    private EventReference startDialogueEvent;

    public void StartDialogue()
    {
        Quaternion destination = Quaternion.LookRotation(playerFeetPointer.Target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Euler(0, destination.eulerAngles.y, 0);
        startDialogueEvent.Invoke(dialogueGraph);
    }
}
