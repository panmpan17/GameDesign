using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


[ExecuteInEditMode]
public class SceneComposer : MonoBehaviour
{
    [SerializeField]
    private string[] sceneNames;

    void Awake()
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying) return;
#endif

        string[] activeScenes = GetActiveSceneNames();

        int length = sceneNames.Length;
        for (int i = 0; i < length; i++)
        {
            string sceneName = sceneNames[i];

            if (!InArray(activeScenes, sceneName))
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }


#if UNITY_EDITOR
    IEnumerator Start()
    {
        if (!EditorApplication.isPlaying)
        {
            yield return null;
            StartEditing();
        }
    }
#endif

    static string[] GetActiveSceneNames()
    {
        int count = SceneManager.sceneCount;
        string[] names = new string[SceneManager.sceneCount];
        for (int i = 0; i < count; i++)
        {
            names[i] = SceneManager.GetSceneAt(i).name;
        }

        return names;
    }

    static bool InArray(string[] array, string item)
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
    public static void StartEditing()
    {
        string[] activeScenes = GetEditorActiveSceneNames();

        SceneComposer composer = FindObjectOfType<SceneComposer>();
        if (composer)
        {
            foreach (string sceneName in composer.sceneNames)
            {
                if (!InArray(activeScenes, sceneName))
                    EditorSceneManager.OpenScene("Assets/Scenes/Levels/" + sceneName + ".unity", OpenSceneMode.Additive);
            }
        }
    }

    static string[] GetEditorActiveSceneNames()
    {
        int count = EditorSceneManager.sceneCount;
        string[] names = new string[EditorSceneManager.sceneCount];
        for (int i = 0; i < count; i++)
        {
            names[i] = EditorSceneManager.GetSceneAt(i).name;
        }

        return names;
    }
#endif
}
