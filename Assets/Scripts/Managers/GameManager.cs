using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        ins = this;

        player.OnDeath += HandlePlayerDeath;
    }

    void HandlePlayerDeath()
    {
        StartCoroutine(C_RevivePlayer());
    }

    IEnumerator C_RevivePlayer()
    {
        yield return new WaitForSeconds(playerReviveTime);
        player.ReviveAtSpawnPoint(spawnPoint);
        playerReviveEvent.Invoke();
    }

    public void ChangePlayerSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }
}
