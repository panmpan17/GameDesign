using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


public class PrefabReplacerWindow : EditorWindow
{
    [MenuItem("MPack/Prefab Replacer")]
    public static void OpenWindow()
    {
        GetWindow<PrefabReplacerWindow>();
    }

    // private GameObject[] prefabs;
    // private GameObject[] meshes;

    // private ReorderableList prefabReorderableList;

    private ReplacerData data;
    private SerializedObject serializedObject;
    private SerializedProperty prefabs;
    private SerializedProperty meshes;
    private Vector2 scrollPosition;


    void OnEnable()
    {
        data = ScriptableObject.CreateInstance<ReplacerData>();
        serializedObject = new SerializedObject(data);
        prefabs = serializedObject.FindProperty("Prefabs");
        meshes = serializedObject.FindProperty("Meshes");
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.PropertyField(prefabs);

        if (GUILayout.Button("Find Mesh"))
        {
            FindMesh();
        }

        EditorGUILayout.PropertyField(meshes);

        if (GUILayout.Button("Repalce"))
        {

            for (int i = 0; i < prefabs.arraySize; i++)
            {
                var prefab = (GameObject)prefabs.GetArrayElementAtIndex(i).objectReferenceValue;
                var mesh = (GameObject)meshes.GetArrayElementAtIndex(i).objectReferenceValue;

                if (prefab && mesh)
                    ReplaceMeshWithPrefab(prefab, mesh);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    void FindMesh()
    {
        meshes.arraySize = prefabs.arraySize;
        for (int i = 0; i < prefabs.arraySize; i++)
        {
            var prefab = (GameObject)prefabs.GetArrayElementAtIndex(i).objectReferenceValue;
            string modalPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab.transform.GetChild(0).gameObject);
            GameObject modal = AssetDatabase.LoadAssetAtPath<GameObject>(modalPath);
            meshes.GetArrayElementAtIndex(i).objectReferenceValue = modal;
        }
    }

    private void ReplaceMeshWithPrefab(GameObject prefab, GameObject mesh)
    {
        Transform referenceTransform = prefab.transform.GetChild(0);
        Vector3 referenceScale = referenceTransform.localScale;
        Quaternion referenceRotation = referenceTransform.rotation;
        GameObject[] variants = PrefabUtility.FindAllInstancesOfPrefab(mesh);

        for (int i = 0; i < variants.Length; i++)
        {
            GameObject oldObject = variants[i];
            Vector3 newScale = Vector3Devide(oldObject.transform.localScale, referenceScale);
            Quaternion newRotation = oldObject.transform.rotation * Quaternion.Inverse(referenceRotation);

            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab, oldObject.transform.parent);
            newObject.transform.position = oldObject.transform.position;
            newObject.transform.rotation = newRotation;
            newObject.transform.localScale = Vector3Devide(oldObject.transform.localScale, referenceScale);

            Undo.DestroyObjectImmediate(oldObject);
            Undo.RegisterCreatedObjectUndo(newObject, "Repalce");
        }
    }

    Vector3 Vector3Devide(Vector3 first, Vector3 second)
    {
        return new Vector3(
            first.x / second.x,
            first.y / second.y,
            first.z / second.z
        );
    }
}


public class ReplacerData : ScriptableObject
{
    public GameObject[] Prefabs;
    public GameObject[] Meshes;
}