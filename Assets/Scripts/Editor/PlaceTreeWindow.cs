using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlaceTreeWindow : EditorWindow
{
    private Transform _parent;
    private GameObject _prefab;
    private Vector2 _size;
    // private Vector3 _centerOffset;

    private Vector3 _step;
    private Vector3 _randomOffset;
    private Vector2 _noiseOffset;
    private float _noiseSize;
    private float _coverage;

    [MenuItem("Terrain/Place Tree")]
    public static void OpenWindow()
    {
        GetWindow<PlaceTreeWindow>();
    }

    void OnGUI()
    {
        _parent = (Transform)EditorGUILayout.ObjectField("Parent", _parent, typeof(Transform), true);
        _prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", _prefab, typeof(GameObject), false);
        _size = EditorGUILayout.Vector2Field("Size", _size);
        _step = EditorGUILayout.Vector2Field("Step", _step);
        _randomOffset = EditorGUILayout.Vector2Field("Random Offset", _randomOffset);

        _noiseOffset = EditorGUILayout.Vector2Field("Noise Offset", _noiseOffset);
        _noiseSize = EditorGUILayout.FloatField("Noise Scale", _noiseSize);
        _coverage = EditorGUILayout.FloatField("Coverage", _coverage);


        if (GUILayout.Button("Clear"))
        {
            ClearObjects();
        }

        if (GUILayout.Button("Generate"))
        {
            ClearObjects();
            Spawn();
        }
    }

    void ClearObjects()
    {
        while (_parent.childCount > 0)
        {
            DestroyImmediate(_parent.GetChild(0).gameObject);
        }
    }

    void Spawn()
    {
        for (float x = 0; x < _size.x; x += _step.x)
        {
            for (float z = 0; z < _size.y; z += _step.y)
            {
                float value = Mathf.PerlinNoise(
                    _noiseOffset.x + (x / _size.x * _noiseSize),
                    _noiseOffset.y + (z / _size.y * _noiseSize));

                if (value < _coverage)
                    continue;

                Vector3 position = new Vector3(x, 0, z);
                position.x += (-_size.x / 2) + Random.Range(-_randomOffset.x, _randomOffset.x);
                position.z += (-_size.y / 2) + Random.Range(-_randomOffset.y, _randomOffset.y);
                GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(_prefab, _parent);
                newObject.transform.position = position;
                newObject.transform.localScale = Vector3.one * value;
            }
        }
    }
}
