using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HCL
{
    public const float HCLgamma = 3;
    public const float HCLy0 = 100;
    public static float HCLmaxL
    {
        get{
            return Mathf.Exp(HCLgamma / HCLy0) - 0.5f;
        }
    }

    public float hue;
    public float chroma;
    public float lightness;
    public float alpha;

    public HCL(float hue, float chroma, float lightness, float alpha)
    {
        this.hue = hue;
        this.chroma = chroma;
        this.lightness = lightness;
        this.alpha = alpha;
    }
}
