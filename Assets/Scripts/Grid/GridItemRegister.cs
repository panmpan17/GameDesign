using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItemRegister : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            GridManager.ins.RegisterChild(child);
        }
    }
}
