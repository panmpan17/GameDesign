using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName="Game/Neighbour Positions")]
public class NeighbourPositions : ScriptableObject
{
    public Vector2Int[] Offsets = new Vector2Int[] { Vector2Int.zero };

    public int EditorGridCount = 3;
}
