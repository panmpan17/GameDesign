using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    [SerializeField]
    private PlayerDetection entranceDetect;
    // TODO: Detect when player enter the area

    [SerializeField]
    private GameObject borderWall;
    [SerializeField]
    private GameObject bossSlime;
    // TODO: Set physic wall to block player, avoid palyer get out of the area
    // TODO: Cutscene, able to skip when player is dead or skip all cutscene
    // TODO: Boss entrance, boss health ui

    // TODO: Reset everthing when player is dead
    // TODO: Kill all small slime that spawn by boss
    // TODO: Reset boss position, health, etc

    // TODO: If player win, then what?

    void Awake()
    {
        borderWall.SetActive(false);
        entranceDetect.OnPlayerEnter += OnPlayerEnterEntrance;
    }

    void OnPlayerEnterEntrance()
    {
        entranceDetect.gameObject.SetActive(false);
        borderWall.SetActive(true);

        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        CameraSwitcher.ins.SwitchTo("BossLand");
        yield return new WaitForSeconds(2f);
        bossSlime.SetActive(true);
        yield return new WaitForSeconds(4f);
        CameraSwitcher.ins.SwitchTo("Walk");
    }
}
