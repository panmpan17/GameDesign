using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MPack
{
    public class ImproveInputField : InputField
    {
        public System.Action onClick;
        public System.Action onSelect;
        public System.Action onDeselect;
        public System.Action onSubmit;

        public override void OnPointerClick(PointerEventData pointerEventData)
        {
            base.OnPointerClick(pointerEventData);
            onClick?.Invoke();
        }

        public override void OnSelect(BaseEventData eventData) {
            base.OnSelect(eventData);
            onSelect?.Invoke();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            onDeselect?.Invoke();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            onSubmit?.Invoke();
        }
    }
}