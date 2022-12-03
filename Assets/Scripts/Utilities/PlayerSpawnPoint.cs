using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerSpawnPoint : MonoBehaviour
{
    public void ChangeThisToPlayerSpawnPoint()
    {
        GameManager.ins.ChangePlayerSpawnPoint(this);
    }

    public void Teleport()
    {
        GameObject player = GameObject.Find("Player");
        GameObject camera = GameObject.Find("Cameras");
        player.transform.position = transform.position;
        player.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        camera.transform.position = transform.position;
    }
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

            Undo.RecordObject(player.transform, "Change Player Position");
            Undo.RecordObject(camera.transform, "Change Player Position");

            player.transform.position = transform.position;
            player.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

            camera.transform.position = transform.position;
        }
    }
}
#endif
