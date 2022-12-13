using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager ins;

    [SerializeField]
    private TransformPointer player;

    [SerializeField]
    private Vector2Int gridSize;
    [SerializeField]
    private Vector2Int gridCount;
    private Vector2Int _mapSize;

    [SerializeField]
    private NeighbourPositions neighborPositions;

    private Vector3 _startPosition;
    private int _playerPositionIndex;

    private Section[] sections;

    [SerializeField]
    private bool drawGizmos;
    [SerializeField]
    private bool drawGizmosSelected;


#region Initial Setup
    void Awake()
    {
        ins = this;
        CalculateSections();
    }

    void Start()
    {
        _playerPositionIndex = GetGirdIndex(GetGridPosition(player.Target.position));
        Vector2Int centerGridPosition = sections[_playerPositionIndex].GridPosition;
        for (int i = 0; i < neighborPositions.Offsets.Length; i++)
        {
            Vector2Int gridPosition = centerGridPosition + neighborPositions.Offsets[i];

            if (!CheckGridPositionIsAvalible(gridPosition))
                continue;

            int index = GetGirdIndex(gridPosition);
            if (index < 0 || index >= sections.Length)
                continue;
            sections[index].Active = true;
        }
    }

    void CalculateSections()
    {
        _mapSize.x = gridSize.x * gridCount.x;
        _mapSize.y = gridSize.y * gridCount.y;

        _startPosition = new Vector3(-_mapSize.x / 2, 0, -_mapSize.y / 2);

        if (gridCount.x == 0 || gridCount.y == 0)
            return;

        Vector3 offset = transform.position;
        sections = new Section[Mathf.FloorToInt(gridCount.x) * Mathf.FloorToInt(gridCount.y)];
        int gridIndex = 0;

        for (int x = 0; x < gridCount.x; x++)
        {
            for (int z = 0; z < gridCount.y; z++)
            {
                Vector3 center = new Vector3(
                    x * gridSize.x + (gridSize.x / 2) - (_mapSize.x / 2) + offset.x,
                    0,
                    z * gridSize.y + (gridSize.y / 2) - (_mapSize.y / 2) + offset.z);

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
        for (int i = 0; i < neighborPositions.Offsets.Length; i++)
        {
            removePositions.Add(centerGridPosition + neighborPositions.Offsets[i]);
        }

        if (newIndex < 0 && newIndex >= sections.Length) return;

        centerGridPosition = sections[newIndex].GridPosition;
        for (int i = 0; i < neighborPositions.Offsets.Length; i++)
        {
            Vector2Int position = centerGridPosition + neighborPositions.Offsets[i];

            if (!CheckGridPositionIsAvalible(position))
                continue;

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


#region Grid Calculate Utilities
    // public Vector2Int GetGridPosition(Vector3 position) => new Vector2Int(Mathf.FloorToInt(position.x - _startPosition.x / gridSize.x), Mathf.FloorToInt(position.z - _startPosition.z / gridSize.y));
    Vector2Int GetGridPosition(Vector3 position)
    {
        Vector3 delta = position - _startPosition - transform.position;
        return new Vector2Int(Mathf.FloorToInt(delta.x / gridSize.x), Mathf.FloorToInt(delta.z / gridSize.y));
    }

    public int GetGirdIndex(Vector2Int gridPosition) => gridPosition.x * gridCount.y + gridPosition.y;

    public bool CheckGridPositionIsAvalible(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0 || gridPosition.y < 0)
            return false;
        if (gridPosition.x >= gridCount.x || gridPosition.y >= gridCount.y)
            return false;

        return true;
    }
#endregion


#region Editor
    void OnValidate() => CalculateSections();

    void OnDrawGizmos()
    {
        if (drawGizmos)
            DrawGizmos();
    }
    void OnDrawGizmosSelected()
    {
        if (drawGizmosSelected)
            DrawGizmos();
    }
    void DrawGizmos()
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
        else
        {
            CalculateSections();
        }
    }
#endregion
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