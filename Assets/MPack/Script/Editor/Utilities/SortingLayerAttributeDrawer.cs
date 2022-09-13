using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MPack
{
    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerAttributeDrawer : PropertyDrawer
    {
        static string[] sortingLayerNames;
        static int[] sortingLayerIDs;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (sortingLayerNames == null)
            {
                GetSortingLayer();
            }

            int index = System.Array.IndexOf(sortingLayerIDs, property.intValue);
            int newIndex = EditorGUI.Popup(position, label.text, index, sortingLayerNames);
            if (newIndex != index)
            {
                property.intValue = sortingLayerIDs[newIndex];
            }
        }

        void GetSortingLayer()
        {
            sortingLayerNames = new string[SortingLayer.layers.Length];
            sortingLayerIDs = new int[SortingLayer.layers.Length];

            for (int i = 0; i < sortingLayerNames.Length; i++)
            {
                sortingLayerNames[i] = SortingLayer.layers[i].name;
                sortingLayerIDs[i] = SortingLayer.layers[i].id;
            }
        }
    }
}