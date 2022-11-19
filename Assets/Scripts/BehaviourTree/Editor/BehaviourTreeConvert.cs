using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Old = TheKiwiCoder;
using New = XnodeBehaviourTree;


public static class BehaviourTreeConvert
{
    [MenuItem("/Game/BehaviourTree 轉換成新系統/Graph", true)]
    public static bool ValidateConvertGraph()
    {
        return Selection.activeObject is Old.BehaviourTree;
    }

    [MenuItem("/Game/BehaviourTree 轉換成新系統/Graph")]
    public static void ConvertGraph()
    {
        var oldTree = (Old.BehaviourTree)Selection.activeObject;
        var newGraph = ScriptableObject.CreateInstance<New.BehaviourTreeGraph>();

        string path = AssetDatabase.GetAssetPath(oldTree);
        AssetDatabase.CreateAsset(newGraph, path.Replace(".asset", "-Graph.asset"));
        AssetDatabase.SaveAssets();

        for (int i = 0; i < oldTree.nodes.Count; i++)
        {
            Old.Node oldNode = oldTree.nodes[i];

            if (oldNode is Old.RootNode)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.RootNode)) as New.RootNode;
                node.name = "RootNode";
                node.position = oldNode.position;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }

#region Composite
            else if (oldNode is Old.Sequencer)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.SequenceNode)) as New.SequenceNode;
                node.name = "SequenceNode";
                node.position = oldNode.position;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is Old.Selector)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.SelectNode)) as New.SelectNode;
                node.name = "SelectNode";
                node.position = oldNode.position;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is Old.InterruptSelector)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.InterruptSelectNode)) as New.InterruptSelectNode;
                node.name = "InterruptSelectNode";
                node.position = oldNode.position;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is Old.RandomSelector)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.RandomSelectNode)) as New.RandomSelectNode;
                node.name = "RandomSelectNode";
                node.position = oldNode.position;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is Old.Parallel)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.ParallelNode)) as New.ParallelNode;
                node.name = "ParallelNode";
                node.position = oldNode.position;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
#endregion

#region Decorate
            else if (oldNode is Old.Repeat)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.RepeatNode)) as New.RepeatNode;
                node.name = "RepeatNode";
                node.position = oldNode.position;

                var oldRepeat = (Old.Repeat)oldNode;
                node.repeatLimit = oldRepeat.repeatLimit;
                node.restartOnFailure = oldRepeat.restartOnFailure;
                node.restartOnSuccess = oldRepeat.restartOnSuccess;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is Old.Timeout)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.TimeoutNode)) as New.TimeoutNode;
                node.name = "RepeatNode";
                node.position = oldNode.position;

                var oldTimeout = (Old.Timeout)oldNode;
                node.duration = oldTimeout.duration;
                node.resultIsSuccess = oldTimeout.resultIsSuccess;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
#endregion

#region Action
            else if (oldNode is ShootTrigger)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.TriggerFire)) as New.TriggerFire;
                node.name = "TriggerFire";
                node.position = oldNode.position;

                var shootTrigger = (ShootTrigger)oldNode;
                node.TriggerGroupIndex = shootTrigger.TriggerGroupIndex;
                node.CarriedParameter = shootTrigger.CarriedParameter;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is FaceTarget)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.FaceTargetNode)) as New.FaceTargetNode;
                node.name = "FaceTargetNode";
                node.position = oldNode.position;

                var faceTarget = (FaceTarget)oldNode;
                node.RotateSpeed = faceTarget.rotateSpeed;
                node.RaycastPoint = faceTarget.raycastPoint;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is Old.Wait)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.WaitNode)) as New.WaitNode;
                node.name = "WaitNode";
                node.position = oldNode.position;

                var wait = (Old.Wait)oldNode;
                node.duration = wait.duration;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is JumpTowards)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.JumpForawrdNode)) as New.JumpForawrdNode;
                node.name = "JumpForwardNode";
                node.position = oldNode.position;

                var wait = (JumpTowards)oldNode;
                node.forwardSpeed = wait.forwardSpeed;
                node.jumpForce = wait.jumpForce;
                node.jumpForceCurve = wait.jumpForceCurve;
                node.jumpTimer = wait.jumpTimer;
                node.landingMoveforawrd = wait.landingMoveforawrd;
                node.extraGravity = wait.extraGravity;
                node.grounLayer = wait.grounLayer;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is SetFixedTargetAsTarget)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.SetFixedPointAsTarget)) as New.SetFixedPointAsTarget;
                node.name = "SetFixedPointAsTarget";
                node.position = oldNode.position;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is SetPlayerPositionAsTarget)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.SetPlayerAsTarget)) as New.SetPlayerAsTarget;
                node.name = "SetPlayerAsTarget";
                node.position = oldNode.position;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is AlignRotationWIthGround)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.AlignRotationNode)) as New.AlignRotationNode;
                node.name = "AlignRotationNode";
                node.position = oldNode.position;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is TargetIsInRange)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.CheckTargetRadiusNode)) as New.CheckTargetRadiusNode;
                node.name = "CheckTargetRadiusNode";
                node.position = oldNode.position;

                var oldTargetInRange = (TargetIsInRange)oldNode;
                node.Max = new MPack.ValueWithEnable<float>(oldTargetInRange.radius, true);
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is TriggerEvent)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.TriggerEventNode)) as New.TriggerEventNode;
                node.name = "TriggerEventNode";
                node.position = oldNode.position;

                var triggerEvent = (TriggerEvent)oldNode;
                node.eventReference = triggerEvent.eventReference;
                node.carriedBoolValue = triggerEvent.carriedBoolValue;
                node.carriedFloatValue = triggerEvent.carriedFloatValue;
                node.carriedIntValue = triggerEvent.carriedIntValue;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is CheckTargetInEyeSight)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.CheckTargetInEyesightNode)) as New.CheckTargetInEyesightNode;
                node.name = "CheckTargetInEyesightNode";
                node.position = oldNode.position;

                var triggerEvent = (CheckTargetInEyeSight)oldNode;
                node.senseRange = triggerEvent.senseRange;
                node.raycastLayers = triggerEvent.raycastLayers;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is RandomFindPositionAsTarget)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.RandomPositionAsTarget)) as New.RandomPositionAsTarget;
                node.name = "RandomPositionAsTarget";
                node.position = oldNode.position;

                var randomFindPosition = (RandomFindPositionAsTarget)oldNode;
                node.startRadius = randomFindPosition.startRadius;
                node.endRadius = randomFindPosition.endRadius;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is CheckPlayerInRange)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.CheckTargetRadiusNode)) as New.CheckTargetRadiusNode;
                node.name = "CheckTargetRadiusNode";
                node.position = oldNode.position;

                var oldPlayerInRange = (CheckPlayerInRange)oldNode;
                node.Min = new MPack.ValueWithEnable<float>(oldPlayerInRange.min, true);
                node.Max = new MPack.ValueWithEnable<float>(oldPlayerInRange.max, true);
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is TriggerShockWave)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.TriggerShockWaveNode)) as New.TriggerShockWaveNode;
                node.name = "TriggerShockWaveNode";
                node.position = oldNode.position;

                var oldTriggerShockWave = (TriggerShockWave)oldNode;
                node.forceSize = oldTriggerShockWave.forceSize;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else if (oldNode is CallFunctionNode)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.CallFunctionNode)) as New.CallFunctionNode;
                node.name = "CallFunctionNode";
                node.position = oldNode.position;

                var oldDefine = (CallFunctionNode)oldNode;
                node.FunctionName = oldDefine.FunctionName;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
#endregion

            else if (oldNode is DefineFunctionNode)
            {
                var node = ScriptableObject.CreateInstance(typeof(New.DefineFunctionNode)) as New.DefineFunctionNode;
                node.name = "DefineFunctionNode";
                node.position = oldNode.position;

                var oldDefine = (DefineFunctionNode)oldNode;
                node.FunctionName = oldDefine.FunctionName;
                newGraph.nodes.Add(node);

                if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(newGraph))) AssetDatabase.AddObjectToAsset(node, newGraph);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.Log(oldNode.name);
            }
        }

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newGraph;
    }


    [MenuItem("/Game/BehaviourTree 轉換成新系統/Component", true)]
    public static bool ValidateConvertComponent()
    {
        if (!Selection.activeGameObject)
            return false;

        var oldRunner = Selection.activeGameObject.GetComponent<SlimeBehaviourTreeRunner>();
        return oldRunner;
    }

    [MenuItem("/Game/BehaviourTree 轉換成新系統/Component")]
    public static void ConvertComponent()
    {
        GameObject gameObject = Selection.activeGameObject;
        var oldRunner = gameObject.GetComponent<SlimeBehaviourTreeRunner>();
        var newRunner = gameObject.AddComponent<New.BehaviourTreeRunner>();

        SerializedObject oldSerialObj = new SerializedObject(oldRunner);
        SerializedObject newSerialObj = new SerializedObject(newRunner);

        newSerialObj.FindProperty("player").objectReferenceValue = oldSerialObj.FindProperty("player").objectReferenceValue;
        newSerialObj.FindProperty("fixedTarget").objectReferenceValue = oldSerialObj.FindProperty("fixedTarget").objectReferenceValue;
        newSerialObj.FindProperty("eyePosition").objectReferenceValue = oldSerialObj.FindProperty("eyePosition").objectReferenceValue;
        newSerialObj.FindProperty("lootTable").objectReferenceValue = oldSerialObj.FindProperty("lootTable").objectReferenceValue;
        newSerialObj.FindProperty("healthChangedEvent").objectReferenceValue = oldSerialObj.FindProperty("healthChangedEvent").objectReferenceValue;

        newSerialObj.FindProperty("sinkHeight").floatValue = oldSerialObj.FindProperty("sinkHeight").floatValue;
        newSerialObj.FindProperty("impulseSource").objectReferenceValue = oldSerialObj.FindProperty("impulseSource").objectReferenceValue;
        newSerialObj.FindProperty("audioSource").objectReferenceValue = oldSerialObj.FindProperty("audioSource").objectReferenceValue;
        newSerialObj.FindProperty("slamSound").objectReferenceValue = oldSerialObj.FindProperty("slamSound").objectReferenceValue;
        newSerialObj.ApplyModifiedPropertiesWithoutUndo();

        // triggerFireGroups
        // sinkTimer
        // OnDeath
    }
}
