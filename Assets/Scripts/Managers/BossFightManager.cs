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
    // TODO: Detect when player enter the area

    [SerializeField]
    private CinemachineVirtualCamera lookBossCam;
    [SerializeField]
    private GameObject borderWall;
    [SerializeField]
    private Transform bossStartPosition;
    [SerializeField]
    private GameObject bossSlimePrefab;
    private GameObject _bossSlime;
    [SerializeField]
    private EventReference slimeHealthShowEvent;
    // TODO: Set physic wall to block player, avoid palyer get out of the area
    // TODO: Cutscene, able to skip when player is dead or skip all cutscene
    // TODO: Boss entrance, boss health ui

    // TODO: Reset everthing when player is dead
    // TODO: Kill all small slime that spawn by boss
    // TODO: Reset boss position, health, etc

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
        GameManager.ins.OnPlayerRevive += ResetBossFight;

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
                overrideBossStartPosition? overrideBossStartPosition.rotation : bossStartPosition.rotation);
            slimeHealthShowEvent.Invoke(true);
            yield break;
        }
#endif

        player.Input.Disable();
        CameraSwitcher.ins.SwitchTo("BossLand");
        lookBossCam.LookAt = bossStartPosition;
        yield return new WaitForSeconds(0.5f);

        _bossSlime = Instantiate(bossSlimePrefab, bossStartPosition.position, bossStartPosition.rotation);
        lookBossCam.LookAt = _bossSlime.transform;
        yield return new WaitForSeconds(4f);

        player.Input.Enable();
        slimeHealthShowEvent.Invoke(true);
        CameraSwitcher.ins.SwitchTo("Walk");
    }

    void ResetBossFight()
    {
        GameManager.ins.OnPlayerRevive -= ResetBossFight;

        borderWall.SetActive(false);
        slimeHealthShowEvent.Invoke(false);
        Destroy(_bossSlime);

        entranceDetect.gameObject.SetActive(true);
    }
}
