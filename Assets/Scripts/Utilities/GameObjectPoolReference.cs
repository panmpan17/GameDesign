using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu(menuName="Game/GameObject Pool")]
public class GameObjectPoolReference : ScriptableObject
{
    public GameObject Prefab;
    public bool CreateCollection;
    public string CollectionName;

    [System.NonSerialized]
    private GameObjectPrefabPool _pool;

    [System.NonSerialized]
    private bool _started = false;

    void OnEnable()
    {
        
    }

    public GameObject Get()
    {
        if (!_started)
        {
            _started = true;
            _pool = new GameObjectPrefabPool(Prefab, CreateCollection, CollectionName);
        }
        return _pool.Get();
    }

    public void Put(GameObject target)
    {
        _pool.Put(target);
    }
}
