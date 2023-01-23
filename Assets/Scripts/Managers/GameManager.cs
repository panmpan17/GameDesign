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

    [Header("Save")]
    [SerializeField]
    private SaveDataReference saveDataReference;
    [SerializeField]
    private EventReference saveDataExtractEvent;
    [SerializeField]
    private EventReference saveDataRestoreEvent;

    private bool _inFight;


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
        saveDataExtractEvent.InvokeEvents += OnSaveDataExtract;
        saveDataRestoreEvent.InvokeEvents += OnSaveDataRestore;
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
        Debug.Log("Register point");
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

#region Saving
    void OnSaveDataExtract()
    {
        if (_inFight)
            saveDataReference.Data.PlayerPosition = _currentSpawnPoint.transform.position;
        else
            saveDataReference.Data.PlayerPosition = player.transform.position;
        saveDataReference.Data.SpawnPointName = _currentSpawnPoint.PointName;
    }

    void OnSaveDataRestore()
    {
        string savePointName = saveDataReference.Data.SpawnPointName;

        Debug.Log("Restore to point: " + savePointName);

        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            if (_spawnPoints[i].PointName == savePointName)
            {
                ChangePlayerSpawnPoint(_spawnPoints[i]);
                break;
            }
        }
        player.InstantTeleportTo(saveDataReference.Data.PlayerPosition);
    }
#endregion

    public void StartFight()
    {
        _inFight = true;
    }

    public void EndFight()
    {
        _inFight = true;
    }

    void OnDestroy()
    {
        saveDataExtractEvent.InvokeEvents -= OnSaveDataExtract;
        saveDataRestoreEvent.InvokeEvents -= OnSaveDataRestore;
    }


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
