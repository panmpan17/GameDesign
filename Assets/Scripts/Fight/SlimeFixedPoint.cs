using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XnodeBehaviourTree;

public class SlimeFixedPoint : AbstractSpawnSlime
{
    [SerializeField]
    private GameObjectList spawnSlimesList;
    [SerializeField]
    private BehaviourTreeRunner[] slimes;
    private BehaviourTreeRunner[] _slimePrefabs;

    void Awake()
    {
        int length = slimes.Length;
        _slimePrefabs = new BehaviourTreeRunner[length];
        for (int i = 0; i < length; i++)
        {
            _slimePrefabs[i] = Instantiate(slimes[i], slimes[i].transform.parent);
            _slimePrefabs[i].gameObject.SetActive(false);
        }
    }

    public override void SetSpawnSlimeList(GameObjectList list) => spawnSlimesList = list;

    public override void TriggerFire()
    {
        foreach (var slime in slimes)
        {
            spawnSlimesList.List.Add(slime.gameObject);
            slime.enabled = true;
        }

        TriggerOnSlimeSpawnedCallback();
    }

    public override void TriggerFireWithParameter(int parameter)
    {}

    public override void ResetFight()
    {
        int length = slimes.Length;
        for (int i = 0; i < length; i++)
        {
            if (slimes[i])
            {
                Destroy(slimes[i].gameObject);
            }

            slimes[i] = Instantiate(_slimePrefabs[i], _slimePrefabs[i].transform.parent);
            slimes[i].gameObject.SetActive(true);
        }
    }

    public override void DestroyFight()
    {
        int length = slimes.Length;
        for (int i = 0; i < length; i++)
        {
            if (slimes[i])
            {
                Destroy(slimes[i].gameObject);
            }
        }
    }
}
