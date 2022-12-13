using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(NeighbourPositions))]
public class NeighbourPositionsEditor : Editor
{
    private static readonly int s_cellSize = 15;
    private SerializedProperty Offsets, EditorGridCount;

    private int _editorGridCount;

    private List<Vector2Int> _offsets;
    private int _offsetLength;

    void OnEnable()
    {
        Offsets = serializedObject.FindProperty("Offsets");
        EditorGridCount = serializedObject.FindProperty("EditorGridCount");
        _editorGridCount = EditorGridCount.intValue;

        RescanOffsets();
    }

    void RescanOffsets()
    {
        _offsetLength = Offsets.arraySize;
        _offsets = new List<Vector2Int>();

        for (int i = 0; i < _offsetLength; i++)
        {
            _offsets.Add(Offsets.GetArrayElementAtIndex(i).vector2IntValue);
        }
    }

    bool IsPositionInOffset(Vector2Int position)
    {
        for (int i = 0; i < _offsetLength; i++)
        {
            if (_offsets[i] == position)
                return true;
        }
        return false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(Offsets);
        if (EditorGUI.EndChangeCheck())
            RescanOffsets();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(EditorGridCount);
        if (EditorGUI.EndChangeCheck())
            _editorGridCount = EditorGridCount.intValue;

        DrawNeighbourCells();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawNeighbourCells()
    {
        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(_editorGridCount * (s_cellSize + 2)));

        Vector2Int center = new Vector2Int(_editorGridCount / 2, _editorGridCount / 2);

        for (int rowIndex = 0; rowIndex < _editorGridCount; rowIndex++)
        {
            Rect cellRect = rect;
            cellRect.width = s_cellSize;
            cellRect.height = s_cellSize;
            cellRect.y = rowIndex * (s_cellSize + 1) + rect.y;

            for (int columnIndex = 0; columnIndex < _editorGridCount; columnIndex++)
            {
                cellRect.x = columnIndex * (s_cellSize + 1) + rect.x;

                Vector2Int position = new Vector2Int(columnIndex - center.x, rowIndex - center.y);

                EditorGUI.BeginChangeCheck();
                bool result = EditorGUI.Toggle(cellRect, IsPositionInOffset(position));
                if (EditorGUI.EndChangeCheck())
                {
                    if (result) AddOffset(position);
                    else RemoveOffset(position);
                }
            }
        }
    }

    void AddOffset(Vector2Int position)
    {
        _offsets.Add(position);
        _offsetLength += 1;
        ApplOffsets();
    }

    void RemoveOffset(Vector2Int position)
    {
        _offsets.Remove(position);
        _offsetLength -= 1;
        ApplOffsets();
    }

    void ApplOffsets()
    {
        Offsets.arraySize = _offsetLength;

        for (int i = 0; i < _offsetLength; i++)
        {
            Offsets.GetArrayElementAtIndex(i).vector2IntValue = _offsets[i];
        }
    }
}
