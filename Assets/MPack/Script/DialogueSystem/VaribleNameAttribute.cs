using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MPack
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false)]
    public class VaribleNameAttribute : PropertyAttribute
    {
        
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(VaribleNameAttribute))]
    public class UnitDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            VaribleNameAttribute labelAttribute = attribute as VaribleNameAttribute;

            AbstractNode node = (AbstractNode)property.serializedObject.targetObject;
            DialogueGraph graph = (DialogueGraph)node.graph;

            string[] variableNames;
            DialogueGraph.VaribleType[] types;
            graph.GetVariableNames(out variableNames, out types);

            int index = System.Array.IndexOf(variableNames, property.stringValue);
            // label.text
            int newIndex = EditorGUI.Popup(position, label.text, index, variableNames);
            if (newIndex != index)
            {
                Undo.RecordObject(node, "");
                property.stringValue = variableNames[newIndex];
            }
            // EditorGUI.PropertyField(position, property, label);
        }
    }
    #endif
}