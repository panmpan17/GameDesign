using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(ItemType))]
public class ItemTypePropertyDrawer : PropertyDrawer
{
    static GUIContent[] itemNames;
    static ItemType[] items;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 20;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        FetchItemNames();

        var selectedItemType = (ItemType)property.objectReferenceValue;
        if (selectedItemType == null)
        {
            int newIndex = EditorGUI.Popup(position, label, -1, itemNames);
            if (newIndex >= 0)
            {
                property.objectReferenceValue = items[newIndex];
            }
        }
        else
        {
            int index = -1;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == selectedItemType)
                {
                    index = i;
                    break;
                }
            }

            int newIndex = EditorGUI.Popup(position, label, index, itemNames);

            if (newIndex != index)
            {
                property.objectReferenceValue = items[newIndex];
            }
        }
    }

    void FetchItemNames()
    {
        if (itemNames == null)
        {
            string[] assets = AssetDatabase.FindAssets("t: ItemType");
            itemNames = new GUIContent[assets.Length];
            items = new ItemType[assets.Length];

            for (int i = 0; i < itemNames.Length; i++)
            {
                ItemType itemType = AssetDatabase.LoadAssetAtPath<ItemType>(AssetDatabase.GUIDToAssetPath(assets[i]));
                itemNames[i] = new GUIContent(itemType.Name);
                items[i] = itemType;
            }
        }
    }
}
