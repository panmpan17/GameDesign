using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using Cinemachine;


public class TeachShootSlimeCore : MonoBehaviour
{
    [SerializeField]
    private TriggerSpawnSlime spawnSlimeTrigger;
    [SerializeField]
    private Transform pointerArrow;
    [SerializeField]
    private string pointSlimeCamera;

    [SerializeField]
    private NPCControl npcControl;
    [SerializeField]
    private VariableStorage variableStorage;

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


        focusTarget = slimeList.List[0].transform;
        // SimpleTutorialHint.ins.FocusAtWorldPosition(slimeList.List[0].transform.position);

        // yield return new WaitForSeconds(1.5f);

        // CameraSwitcher.ins.SwitchTo("Walk");

        variableStorage.Set("TutorialSlimeIsSpawned", true);
        npcControl.StartDialogue();
    }

    void OpenFocusFrame()
    {
        SimpleTutorialHint.ins.FocusAtWorldPosition(focusTarget.position);
    }

    void CloseFocusFrame()
    {
        SimpleTutorialHint.ins.CloseFocus();
        CameraSwitcher.ins.SwitchTo("Walk");
    }
}
