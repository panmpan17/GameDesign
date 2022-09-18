using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Game/Item Type")]
public class ItemType : ScriptableObject
{
    public string Name;
    public Sprite Sprite;
    [Multiline]
    public string Description;
}


// [AttributeUsage(AttributeTargets.Field, AllowMultiple=false)]
// public class LayerAttribute : PropertyAttribute
// {
// }