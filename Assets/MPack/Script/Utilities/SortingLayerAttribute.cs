using System;
using UnityEngine;

namespace MPack
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false)]
    public class SortingLayerAttribute : PropertyAttribute
    {
    }
}