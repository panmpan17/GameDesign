using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

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
        transform.rotation = Quaternion.LookRotation(playerFeetPointer.Target.position - transform.position, Vector3.up);
        startDialogueEvent.Invoke(dialogueGraph);
    }
}
