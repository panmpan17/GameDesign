using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private readonly static Vector2Int[] GridPositionOffset = new Vector2Int[] {
        new Vector2Int(-1, -1),
        Vector2Int.up,
        new Vector2Int(1, -1),
        Vector2Int.left,
        Vector2Int.zero,
        Vector2Int.right,
        new Vector2Int(-1, 1),
        Vector2Int.down,
        new Vector2Int(1, 1),
    };
    public static GridManager ins;

    [SerializeField]
    private TransformPointer player;

    [SerializeField]
    private Vector2Int mapSize;
    [SerializeField]
    private Vector2 gridSize;
    private Vector2Int _gridCount;

    private Vector3 _startPosition;
    private int _playerPositionIndex;

    private Section[] sections;

    [SerializeField]
    private bool drawGizmos;


#region Initial Setup
    void Awake()
    {
        ins = this;
        _startPosition = new Vector3(-mapSize.x / 2, 0, -mapSize.y / 2);
        CalculateSections();

        _playerPositionIndex = GetGirdIndex(GetGridPosition(player.Target.position));
        Vector2Int centerGridPosition = sections[_playerPositionIndex].GridPosition;
        for (int i = 0; i < GridPositionOffset.Length; i++)
        {
            int index = GetGirdIndex(centerGridPosition + GridPositionOffset[i]);
            if (index < 0 || index >= sections.Length)
                continue;
            sections[index].Active = true;
        }
    }

    void CalculateSections()
    {
        _gridCount = new Vector2Int(
            Mathf.FloorToInt(mapSize.x / gridSize.x),
            Mathf.FloorToInt(mapSize.y / gridSize.y));
        if (gridSize.x == 0 || gridSize.y == 0)
            return;

        sections = new Section[Mathf.FloorToInt(_gridCount.x) * Mathf.FloorToInt(_gridCount.y)];
        int gridIndex = 0;

        for (int x = 0; x < _gridCount.x; x++)
        {
            for (int z = 0; z < _gridCount.y; z++)
            {
                Vector3 center = new Vector3(
                    x * gridSize.x + (gridSize.x / 2) - (mapSize.x / 2),
                    0,
                    z * gridSize.y + (gridSize.y / 2) - (mapSize.y / 2));

                sections[gridIndex++] = new Section { Center = center, GridPosition = new Vector2Int(x, z) };
            }
        }
    }

    public void RegisterChild(Transform child)
    {
        Section section = sections[GetGirdIndex(GetGridPosition(child.position))];
        section.SetActiveEvent += child.gameObject.SetActive;
        child.gameObject.SetActive(section.Active);
    }
    #endregion

    void LateUpdate()
    {
        int newIndex = GetGirdIndex(GetGridPosition(player.Target.position));

        if (_playerPositionIndex != newIndex)
        {
            SwitchToNewSection(newIndex);
        }
    }

    void SwitchToNewSection(int newIndex)
    {
        List<Vector2Int> removePositions = new List<Vector2Int>(9);

        Vector2Int centerGridPosition = sections[_playerPositionIndex].GridPosition;
        for (int i = 0; i < GridPositionOffset.Length; i++)
        {
            removePositions.Add(centerGridPosition + GridPositionOffset[i]);
        }

        centerGridPosition = sections[newIndex].GridPosition;
        for (int i = 0; i < GridPositionOffset.Length; i++)
        {
            Vector2Int position = centerGridPosition + GridPositionOffset[i];


            int index = GetGirdIndex(position);
            if (index < 0 || index >= sections.Length)
                continue;
            sections[index].Active = true;

            removePositions.Remove(position);
        }

        for (int i = 0; i < removePositions.Count; i++)
        {
            int index = GetGirdIndex(removePositions[i]);
            if (index < 0 || index >= sections.Length)
                continue;
            sections[index].Active = false;
        }

        _playerPositionIndex = newIndex;
    }

    // public Vector2Int GetGridPosition(Vector3 position) => new Vector2Int(Mathf.FloorToInt(position.x - _startPosition.x / gridSize.x), Mathf.FloorToInt(position.z - _startPosition.z / gridSize.y));
    Vector2Int GetGridPosition(Vector3 position)
    {
        Vector3 delta = position - _startPosition;
        return new Vector2Int(Mathf.FloorToInt(delta.x / gridSize.x), Mathf.FloorToInt(delta.z / gridSize.y));
    }
    public int GetGirdIndex(Vector2Int gridPosition) => gridPosition.x * _gridCount.y + gridPosition.y;



    void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        if (sections != null)
        {
            Vector3 grid = new Vector3(gridSize.x, 1, gridSize.y);
            for (int i = 0; i < sections.Length; i++)
            {
                if (_playerPositionIndex == i)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(sections[i].Center, grid);
                }
                else
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(sections[i].Center, grid);
                }
            }
            Gizmos.DrawSphere(_startPosition, 0.1f);
        }
    }
}


public class Section
{
    public Vector3 Center;
    public Vector2Int GridPosition;
    public event System.Action<bool> SetActiveEvent;
    public bool Active
    {
        get => _isActive;
        set
        {
            if (_isActive == value)
                return;
            SetActiveEvent?.Invoke(value);
            _isActive = value;
        }
    }
    private bool _isActive = false;
}