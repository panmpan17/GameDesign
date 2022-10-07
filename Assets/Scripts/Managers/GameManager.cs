using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour player;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private float playerReviveTime;

    void Awake()
    {
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
    }
}
