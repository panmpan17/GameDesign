using System.Collections;
using System.Collections.Generic;
using MPack;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _ins;
    public static GameManager ins {
        get {
            if (!_ins)
                _ins = GameObject.Find("GameManager").GetComponent<GameManager>();
            return _ins;
        }
    }

    [SerializeField]
    private PlayerBehaviour player;
    public PlayerBehaviour Player => player;
    private PlayerSpawnPoint _currentSpawnPoint;
    private List<PlayerSpawnPoint> _spawnPoints = new List<PlayerSpawnPoint>();


    [SerializeField]
    private float playerReviveTime;
    [SerializeField]
    private EventReference playerReviveEvent;


#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool overrideStartPoint;
    [SerializeField]
    private string startPointName;
#endif

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
        DeadMessage.ins.StartFadeIn();
        yield return new WaitForSeconds(playerReviveTime);
        player.ReviveAtSpawnPoint(_currentSpawnPoint);
        playerReviveEvent.Invoke();

        DeadMessage.ins.StartFadeOut();
    }


#region Player spawn point
    public void ChangePlayerSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        _currentSpawnPoint = spawnPoint;
        _currentSpawnPoint.OnChangeToSpawnPoint();
    }

    public void RegisterSpawnPoint(PlayerSpawnPoint spawnPoint)
    {
        _spawnPoints.Add(spawnPoint);

#if UNITY_EDITOR
        if (overrideStartPoint)
        {
            if (spawnPoint.PointName == startPointName)
                ChangePlayerSpawnPoint(spawnPoint);
            return;
        }
#endif
        if (spawnPoint.PointName == "Start")
            ChangePlayerSpawnPoint(spawnPoint);
    }
#endregion


    [ConsoleCommand("tp :string")]
    public static void LoadPoint1(string name)
    {
        if (name.ToLower() == "last")
        {
            GameManager.ins.Player.InstantTeleportTo(GameManager.ins._currentSpawnPoint.transform.position);
            return;
        }
        foreach (PlayerSpawnPoint point in ins._spawnPoints)
        {
            if (point.PointName.ToLower() == name.ToLower())
            {
                GameManager.ins._currentSpawnPoint = point;
                GameManager.ins.Player.InstantTeleportTo(point.transform.position);
            }
        }
    }
}
