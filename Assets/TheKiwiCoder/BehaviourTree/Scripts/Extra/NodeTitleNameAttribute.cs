using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
public class NodeTitleNameAttribute : PropertyAttribute
{
    public string Title;

    public NodeTitleNameAttribute(string title)
    {
        Title = title;
    }
}
