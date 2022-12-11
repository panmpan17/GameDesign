using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using Cinemachine;


public class TeachShootSlimeCore : MonoBehaviour
{
    [Header("NPC")]
    [SerializeField]
    private NPCControl npcControl;
    [SerializeField]
    private VariableStorage variableStorage;

    [Header("Spawn slime")]
    [SerializeField]
    private TriggerSpawnSlime spawnSlimeTrigger;
    [SerializeField]
    private string pointSlimeCamera;

    [Header("Mark Slime Core")]
    [SerializeField]
    private Transform arrow;
    [SerializeField]
    private EventReference focusOpenEvent;
    [SerializeField]
    private EventReference focusCloseEvent;
    private Transform focusTarget;

    void Awake()
    {
        focusOpenEvent.InvokeEvents += OpenFocusFrame;
        focusCloseEvent.InvokeEvents += CloseFocusFrame;
    }

    
    public void StartTutorial()
    {
        StartCoroutine(C_TutorialProcess());
    }

    IEnumerator C_TutorialProcess()
    {
        PlayerBehaviour player = GameManager.ins.Player;
        player.Input.Disable();

        GameObjectList slimeList = ScriptableObject.CreateInstance<GameObjectList>();
        slimeList.List = new List<GameObject>();

        CameraSwitcher.ins.SwitchTo(pointSlimeCamera);

        yield return new WaitForSeconds(1);

        spawnSlimeTrigger.SetSpawnSlimeList(slimeList);
        spawnSlimeTrigger.TriggerFire();
        yield return new WaitForSeconds(2);

        var core = slimeList.List[0].GetComponentInChildren<SlimeCore>();
        focusTarget = core.transform;

        variableStorage.Set("TutorialSlimeIsSpawned", true);
        npcControl.StartDialogue();
    }

    void OpenFocusFrame()
    {
        SimpleTutorialHint.ins.FocusAtWorldPosition(focusTarget.position);
        // pointerArrow.SetTarget(focusTarget);
        arrow.gameObject.SetActive(true);
        arrow.position = focusTarget.position;
    }

    void CloseFocusFrame()
    {
        SimpleTutorialHint.ins.CloseFocus();
        CameraSwitcher.ins.SwitchTo("Walk");
    }
}
