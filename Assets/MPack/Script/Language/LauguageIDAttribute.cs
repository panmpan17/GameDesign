using System;
using UnityEngine;

namespace MPack
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false)]
    public class LauguageIDAttribute : PropertyAttribute
    {
        public bool IsTextArea;
        public bool CanCollapse;

        public LauguageIDAttribute(bool isTextArea=false, bool canCollapse=false)
        {
            IsTextArea = isTextArea;
            CanCollapse = canCollapse;
        }
    }
}