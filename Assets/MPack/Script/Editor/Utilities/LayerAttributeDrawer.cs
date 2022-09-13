using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace MPack
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerAttributeDrawer : PropertyDrawer
    {

        static string[] layerNames;
        static int[] layerIndex;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (layerNames == null)
            {
                GetSortingLayer();
            }

            int index = System.Array.IndexOf(layerIndex, property.intValue);
            int newIndex = EditorGUI.Popup(position, label.text, index, layerNames);
            if (newIndex != index)
            {
                property.intValue = layerIndex[newIndex];
            }
        }

        void GetSortingLayer()
        {
            // layerNames = new string[Physics2D.layer];
            // layerIndex = new int[SortingLayer.layers.Length];
            List<string> names = new List<string>();
            List<int> index = new List<int>();

            for (int i = 0; i <= 31; i++)
            {
                string name = LayerMask.LayerToName(i);

                if (name.Length > 0)
                {
                    names.Add(name);
                    index.Add(i);
                }
            }

            layerNames = names.ToArray();
            layerIndex = index.ToArray();
        }
    }
}