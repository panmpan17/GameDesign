using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CanvasUtilities
{
    public static Vector3 ViewportToCanvasAnchoredPoint(this Canvas canvas, Vector3 viewportPosition)
    {
        var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
        var canvasRect = canvas.GetComponent<RectTransform>();
        var scale = canvasRect.sizeDelta;
        return Vector3.Scale(centerBasedViewPortPosition, scale);
    }
}
