using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridItemRegister : MonoBehaviour
{
    void Start()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            GridManager.ins.RegisterChild(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
