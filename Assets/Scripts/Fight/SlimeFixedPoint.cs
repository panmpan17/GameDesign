using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeFixedPoint : MonoBehaviour
{

    [SerializeField]
    private GameObjectList spawnSlimesList;
    [SerializeField]
    private XnodeBehaviourTree.BehaviourTreeRunner[] slimes;

    public event System.Action OnSlimeSpawnedCallback;

    public void SetSpawnSlimeList(GameObjectList list) => spawnSlimesList = list;

    public void TriggerFire()
    {
        foreach (var slime in slimes)
        {
            spawnSlimesList.List.Add(slime.gameObject);
            slime.enabled = true;
        }

        OnSlimeSpawnedCallback?.Invoke();
    }
}
