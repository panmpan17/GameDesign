using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossFightManager : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour player;
    [SerializeField]
    private PlayerDetection entranceDetect;

    [SerializeField]
    private CinemachineVirtualCamera lookBossCam;
    [SerializeField]
    private GameObject borderWall;
    [SerializeField]
    private Transform bossStartPosition;
    [SerializeField]
    private GameObject bossSlimePrefab;
    private SlimeBehaviourTreeRunner _bossSlime;
    [SerializeField]
    private EventReference slimeHealthShowEvent;
    [SerializeField]
    private EventReference playerReviveEvent;

    // TODO: Cutscene, able to skip when player is dead or skip all cutscene

    // TODO: If player win, then what?

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool startFightImmediately;
    [SerializeField]
    private Transform overrideBossStartPosition;
#endif

    void Awake()
    {
        borderWall.SetActive(false);
        entranceDetect.OnPlayerEnterEvent += OnPlayerEnterEntrance;
    }

    void OnPlayerEnterEntrance()
    {
        playerReviveEvent.InvokeEvents += ResetBossFight;

        entranceDetect.gameObject.SetActive(false);
        borderWall.SetActive(true);

        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
#if UNITY_EDITOR
        if (startFightImmediately) {
            _bossSlime = Instantiate(
                bossSlimePrefab,
                overrideBossStartPosition? overrideBossStartPosition.position : bossStartPosition.position,
                overrideBossStartPosition? overrideBossStartPosition.rotation : bossStartPosition.rotation).GetComponent<SlimeBehaviourTreeRunner>();
            slimeHealthShowEvent.Invoke(true);
            yield break;
        }
#endif

        player.Input.Disable();
        CameraSwitcher.ins.SwitchTo("BossLand");
        lookBossCam.LookAt = bossStartPosition;
        yield return new WaitForSeconds(0.5f);

        _bossSlime = Instantiate(bossSlimePrefab, bossStartPosition.position, bossStartPosition.rotation).GetComponent<SlimeBehaviourTreeRunner>();
        lookBossCam.LookAt = _bossSlime.transform;
        yield return new WaitForSeconds(4f);

        player.Input.Enable();
        slimeHealthShowEvent.Invoke(true);
        _bossSlime.UpdateHealth();
        CameraSwitcher.ins.SwitchTo("Walk");
    }

    void ResetBossFight()
    {
        playerReviveEvent.InvokeEvents -= ResetBossFight;

        borderWall.SetActive(false);
        slimeHealthShowEvent.Invoke(false);
        Destroy(_bossSlime.gameObject);

        // TODO: Kill all small slime that spawn by boss

        entranceDetect.gameObject.SetActive(true);
    }
}
