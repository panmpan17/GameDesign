using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHide : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform treesParent;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private Vector2Int mapSize;
    [SerializeField]
    private Vector2 gridSize;
    private Vector2Int _gridCount;

    private Vector3 _startPosition;
    private int _playerPositionIndex;

    [SerializeField]
    private int count;
    private Grid[] grids;

    [SerializeField]
    private bool drawGizmos;

    void Start()
    {
        _startPosition = new Vector3(-mapSize.x / 2, 0, -mapSize.y / 2);
        count = treesParent.childCount;
        CalculateGrid();

        for (int i = 0; i < treesParent.childCount; i++)
        {
            Transform child = treesParent.GetChild(i);
            grids[GetGirdIndex(GetGridPosition(child.position))].SetActiveEvent += child.gameObject.SetActive;
        }

        for (int i = 0; i < grids.Length; i++)
        {
            grids[i].SetActive(false);
        }

        _playerPositionIndex = GetGirdIndex(GetGridPosition(player.position));
        grids[_playerPositionIndex].SetActive(true);
    }

    void CalculateGrid()
    {
        _gridCount = new Vector2Int(
            Mathf.FloorToInt(mapSize.x / gridSize.x),
            Mathf.FloorToInt(mapSize.y / gridSize.y));
        if (gridSize.x == 0 || gridSize.y == 0)
            return;

        grids = new Grid[Mathf.FloorToInt(_gridCount.x) * Mathf.FloorToInt(_gridCount.y)];
        int gridIndex = 0;

        for (int x = 0; x < _gridCount.x; x++)
        {
            for (int z = 0; z < _gridCount.y; z++)
            {
                Vector3 center = new Vector3(
                    x * gridSize.x + (gridSize.x / 2) - (mapSize.x / 2),
                    0,
                    z * gridSize.y + (gridSize.y / 2) - (mapSize.y / 2));

                grids[gridIndex++] = new Grid{ Center = center, GridPosition=new Vector2Int(x, z) };
            }
        }
    }

    void LateUpdate()
    {
        int newIndex = GetGirdIndex(GetGridPosition(player.position));

        if (_playerPositionIndex != newIndex)
        {
            grids[_playerPositionIndex].SetActive(false);
            _playerPositionIndex = newIndex;
            grids[_playerPositionIndex].SetActive(true);
        }
    }

    Vector2Int GetGridPosition(Vector3 position)
    {
        Vector3 delta = position - _startPosition;
        return new Vector2Int(Mathf.FloorToInt(delta.x / gridSize.x), Mathf.FloorToInt(delta.z / gridSize.y));
    }

    int GetGirdIndex(Vector2Int gridPosition)
    {
        return gridPosition.x * _gridCount.y + gridPosition.y;
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        if (grids != null)
        {
            Vector3 grid = new Vector3(gridSize.x, 1, gridSize.y);
            for (int i = 0; i < grids.Length; i++)
            {
                if (_playerPositionIndex == i)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(grids[i].Center, grid);
                }
                else
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(grids[i].Center, grid);
                }
            }
            Gizmos.DrawSphere(_startPosition, 0.1f);
        }
    }

    public struct Grid
    {
        public Vector3 Center;
        public Vector2Int GridPosition;
        public event System.Action<bool> SetActiveEvent;

        public void SetActive(bool status)
        {
            SetActiveEvent?.Invoke(status);
        }
    }
}
