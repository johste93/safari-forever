using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSI
{
    public float hue;
    public float saturation;
    public float intensity;
    public float alpha;

    public HSI(float hue, float saturation, float intensity, float alpha)
    {
        this.hue = hue;
        this.saturation = saturation;
        this.intensity = intensity;
        this.alpha = alpha;
    }
}
