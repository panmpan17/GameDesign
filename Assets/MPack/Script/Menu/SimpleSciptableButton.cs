using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace MPack
{
    public class SimpleSciptableButton : MonoBehaviour, IPointerClickHandler
    {
        [System.NonSerialized]
        public int identifiedIndex;

        public System.Action<SimpleSciptableButton, PointerEventData> onClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(this, eventData);
        }
    }
}