using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


public class AnimationCombine : EditorWindow
{
    [MenuItem("MPack/Animation Combine")]
    public static void OepnMenu()
    {
        GetWindow<AnimationCombine>();
    }

    private GameObject gameObject;
    private AnimationClip[] animations;

    private AnimationClip clip;
    private ReorderableList animationList;

    // void OnEnable()
    // {
        // animationList = new ReorderableList(animations, typeof(AnimationClip), true, true, true, true);
    // }

    void OnGUI()
    {
        // EditorGUI.BeginChangeCheck();
        // animations = (AnimationClip[])EditorGUILayout.ObjectField(animations, typeof(AnimationClip[]), false);
        // EditorGUILayout.field

        // if (EditorGUI.EndChangeCheck())
        // {
        //     animationList.list = AnimationUtility.GetAnimationClips(gameObject);
        //     Debug.Log(animations.Length);
        //     // animationList.list =
        //     return;
        // }

        clip = (AnimationClip) EditorGUILayout.ObjectField(clip, typeof(AnimationClip), false);

        // animationList.DoLayoutList();

        if (GUILayout.Button("Combine"))
        {
            Combine();
        }
    }

    void Combine()
    {
        AnimationClip animation = clip;
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(animation);
        // Debug.Log(settings.)
        // AnimationUtility.GetAnimatableBindings()
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(animation);
        Debug.Log(bindings.Length);
        Debug.Log(bindings[0].path);
        Debug.Log(bindings[0].propertyName);
        // animation.
    }
}
