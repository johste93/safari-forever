using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class CanvasExtensions
{
    public static float GetWidth(this Canvas canvas)
    {
        return canvas.GetComponent<RectTransform>().rect.width;
    }

    public static float GetHeight(this Canvas canvas)
    {
        return canvas.GetComponent<RectTransform>().rect.height;
    }

    public static float GetCanvasAspect(this Canvas canvas)
    {
        RectTransform rT = canvas.GetComponent<RectTransform>();
        return rT.rect.width / rT.rect.height;
    }
}
