using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectableExtendedEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private GraphicTargetManipulate[] graphicTargetManipulates;

    void Awake()
    {
        foreach (GraphicTargetManipulate graphicTarget in graphicTargetManipulates)
        {
            if (graphicTarget.ActivateWhenHovered)
                graphicTarget.Graphic.enabled = false;
        }
    }


    public void OnSelect(BaseEventData eventData)
    {
        // Debug.Log("on select");
        // throw new System.NotImplementedException();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // Debug.Log("on deselect");
        // throw new System.NotImplementedException();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (GraphicTargetManipulate graphicTarget in graphicTargetManipulates)
        {
            if (graphicTarget.ActivateWhenHovered)
                graphicTarget.Graphic.enabled = true;
        }
        // throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        foreach (GraphicTargetManipulate graphicTarget in graphicTargetManipulates)
        {
            if (graphicTarget.ActivateWhenHovered)
                graphicTarget.Graphic.enabled = false;
        }
        // throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // throw new System.NotImplementedException();
    }


    [System.Serializable]
    public struct GraphicTargetManipulate
    {
        public Graphic Graphic;

        public bool ActivateWhenHovered;
    }
}
