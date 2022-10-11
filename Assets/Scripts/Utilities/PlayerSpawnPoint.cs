using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerSpawnPoint : MonoBehaviour
{
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerSpawnPoint))]
public class PlayerSpawnPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Teleport Player"))
        {
            Transform transform = ((PlayerSpawnPoint)target).transform;

            GameObject player = GameObject.Find("Player");
            GameObject camera = GameObject.Find("Cameras");
            player.transform.position = transform.position;
            camera.transform.position = transform.position;
        }
    }
}
#endif
