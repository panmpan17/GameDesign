using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


public class SceneComposer : MonoBehaviour
{
    [SerializeField]
    private string[] sceneNames;

    void Awake()
    {
        string[] activeScenes = GetActiveSceneNames();

        int length = sceneNames.Length;
        for (int i = 0; i < length; i++)
        {
            string sceneName = sceneNames[i];

            if (!InArray(activeScenes, sceneName))
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    string[] GetActiveSceneNames()
    {
        int count = SceneManager.sceneCount;
        string[] names = new string[SceneManager.sceneCount];
        for (int i = 0; i < count; i++)
        {
            names[i] = SceneManager.GetSceneAt(i).name;
        }

        return names;
    }

    bool InArray(string[] array, string item)
    {
        int length = array.Length;
        for (int i = 0; i < length; i++)
        {
            if (array[i] == item)
                return true;
        }
        return false;
    }

#if UNITY_EDITOR
    [MenuItem("Game/開始編輯", false, 0)]
    public static void StartEditing()
    {
        if (EditorSceneManager.GetActiveScene().name != "MainGame")
            EditorSceneManager.OpenScene("Assets/Scenes/MainGame.unity");

        SceneComposer composer = FindObjectOfType<SceneComposer>();
        if (composer)
        {
            foreach (string sceneName in composer.sceneNames)
            {
                EditorSceneManager.OpenScene("Assets/Scenes/Levels/" + sceneName + ".unity", OpenSceneMode.Additive);
            }
        }
    }
#endif
}
