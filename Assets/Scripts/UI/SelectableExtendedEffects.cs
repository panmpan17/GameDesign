using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectableExtendedEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private GraphicTargetManipulate[] graphicTargetManipulates;

    private Color[] originalColors;
    private bool _isSelected;
    private bool _isHovered;
    private bool _isPressed;

    void Awake()
    {
        originalColors = new Color[graphicTargetManipulates.Length];
        for (int i = 0; i < graphicTargetManipulates.Length; i++)
        {
            GraphicTargetManipulate graphicTarget = graphicTargetManipulates[i];
            if (graphicTarget.ActivateWhenHovered || graphicTarget.ActivateWhenSelected)
                graphicTarget.Graphic.enabled = false;

            originalColors[i] = graphicTarget.Graphic.color;
        }
    }


    public void OnSelect(BaseEventData eventData)
    {
        _isSelected = true;
        UpdateGraphic();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
        UpdateGraphic();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        UpdateGraphic();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        UpdateGraphic();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        UpdateGraphic();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
        UpdateGraphic();
    }

    void UpdateGraphic()
    {
        for (int i = 0; i < graphicTargetManipulates.Length; i++)
        {
            GraphicTargetManipulate graphicTarget = graphicTargetManipulates[i];

            if (graphicTarget.ActivateWhenHovered && graphicTarget.ActivateWhenSelected)
                graphicTarget.Graphic.enabled = _isHovered || _isSelected;
            else if (graphicTarget.ActivateWhenHovered)
                graphicTarget.Graphic.enabled = _isHovered;
            else if (graphicTarget.ActivateWhenSelected)
                graphicTarget.Graphic.enabled = _isSelected;

            if (!graphicTarget.Graphic.enabled)
                continue;

            if (graphicTarget.ChangeColorWhenPressed && _isPressed)
            {
                graphicTarget.Graphic.color = graphicTarget.PressedColor;
                continue;
            }

            if (graphicTarget.ChangeColorWhenHovered && _isHovered)
            {
                graphicTarget.Graphic.color = graphicTarget.HoveredColor;
                continue;
            }

            if (graphicTarget.ChangeColorWhenSelected && _isSelected)
            {
                graphicTarget.Graphic.color = graphicTarget.SelectedColor;
                continue;
            }

            graphicTarget.Graphic.color = originalColors[i];
        }
    }


    [System.Serializable]
    public struct GraphicTargetManipulate
    {
        public Graphic Graphic;

        public bool ActivateWhenHovered;
        public bool ChangeColorWhenHovered;
        public Color HoveredColor;

        public bool ChangeColorWhenPressed;
        public Color PressedColor;

        public bool ActivateWhenSelected;
        public bool ChangeColorWhenSelected;
        public Color SelectedColor;
    }
}
