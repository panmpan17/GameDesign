using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class PrefabPoolManager : MonoBehaviour
{
    [SerializeField]
    private GameObjectPoolReference[] gameObjectPoolReferences;

    void Awake()
    {
        for (int i = 0; i < gameObjectPoolReferences.Length; i++)
        {
            GameObjectPoolReference poolReference = gameObjectPoolReferences[i];
            gameObjectPoolReferences[i].CreatePool();
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < gameObjectPoolReferences.Length; i++)
        {
            gameObjectPoolReferences[i].ClearPool();
        }
    }
}
