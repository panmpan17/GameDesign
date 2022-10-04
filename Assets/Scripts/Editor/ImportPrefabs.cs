using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImportPrefabs : MonoBehaviour
{
    public static string Path = "Assets/Scripts/Editor/PrefabList.asset";

    [MenuItem("/Game/匯入 Prefabs", false, 0)]
    public static void Imports()
    {
        // ImportPrefabList list = ScriptableObject.CreateInstance<ImportPrefabList>();
        // AssetDatabase.CreateAsset(list, Path);
        // AssetDatabase.SaveAssets();

        var list = AssetDatabase.LoadAssetAtPath<GameObjectList>(Path);

        for (int i = 0; i < list.List.Length; i++)
        {
            GameObject prefab = list.List[i];
            GameObject existed = GameObject.Find(prefab.name);
            if (existed)
                continue;

            Undo.RegisterCreatedObjectUndo(PrefabUtility.InstantiatePrefab(prefab), "Create " + prefab.name);
        }
    }
}
