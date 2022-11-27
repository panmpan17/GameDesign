using System.Collections;
using System.Collections.Generic;
using MPack;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager ins;

    [SerializeField]
    private PlayerBehaviour player;
    [SerializeField]
    private PlayerSpawnPoint spawnPoint;

    [SerializeField]
    private float playerReviveTime;
    [SerializeField]
    private EventReference playerReviveEvent;


    [SerializeField]
    private PlayerSpawnPoint[] spawnPoints;

    void Awake()
    {
        ins = this;

        player.OnDeath += HandlePlayerDeath;

        if (s_loadPoint)
        {
            s_loadPoint = false;
            spawnPoints[s_pointIndex].ChangeThisToPlayerSpawnPoint();
            spawnPoints[s_pointIndex].Teleport();
        }
    }

    void HandlePlayerDeath()
    {
        StartCoroutine(C_RevivePlayer());
    }

    IEnumerator C_RevivePlayer()
    {
        DeadMessage.ins.StartFadeIn();
        yield return new WaitForSeconds(playerReviveTime);
        player.ReviveAtSpawnPoint(spawnPoint);
        playerReviveEvent.Invoke();

        DeadMessage.ins.StartFadeOut();
    }

    public void ChangePlayerSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }


    private static bool s_loadPoint = false;
    private static int s_pointIndex = 0;

    [ConsoleCommand("load1")]
    public static void LoadPoint1()
    {
        s_loadPoint = true;
        s_pointIndex = 0;
        LoadScene.ins.Load(SceneManager.GetActiveScene().name);
    }

    [ConsoleCommand("load2")]
    public static void LoadPoint2()
    {
        s_loadPoint = true;
        s_pointIndex = 1;
        LoadScene.ins.Load(SceneManager.GetActiveScene().name);
    }
}
