using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSpot : MonoBehaviour
{
    public void Teleport(Transform transform)
    {
        transform.position = this.transform.position;
        transform.rotation = this.transform.rotation;
    }
}
