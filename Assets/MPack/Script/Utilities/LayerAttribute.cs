using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MPack
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false)]
    public class LayerAttribute : PropertyAttribute
    {
    }
}