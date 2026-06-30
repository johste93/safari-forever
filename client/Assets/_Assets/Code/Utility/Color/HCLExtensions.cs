using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HCLExtensions
{
    public static HCL RGBtoHCL(float r, float g, float b, float a)
    {
        Color c = new Color(r, g, b, a);   
        return c.ToHCL();
    }

    public static HCL ToHCL(this Color RGBA)
    {
        HCL hCL = new HCL();
        hCL.alpha = RGBA.a;
        float H = 0;
        float U = Mathf.Min(RGBA.r, Mathf.Min(RGBA.g, RGBA.b));
        float V = Mathf.Max(RGBA.r, Mathf.Max(RGBA.g, RGBA.b));
        float Q = HCL.HCLgamma / HCL.HCLy0;
        hCL.chroma = V - U;
        if (hCL.chroma != 0)
        {
        H = Mathf.Atan2(RGBA.g - RGBA.b, RGBA.r - RGBA.g) / Mathf.PI;
        Q *= U / V;
        }
        Q = Mathf.Exp(Q);
        hCL.hue = MathIsHard.Frac(H / 2 - Mathf.Min(MathIsHard.Frac(H), MathIsHard.Frac(-H)) / 6);
        hCL.chroma *= Q;
        hCL.lightness = Mathf.Lerp(-U, V, Q) / (HCL.HCLmaxL * 2);
        return hCL;
    }

    public static Color ToRGB(this HCL hCL)
    {
        Vector3 RGB = new Vector3();
        if (hCL.lightness != 0)
        {
            float H = hCL.hue;
            float C = hCL.chroma;
            float L = hCL.lightness * HCL.HCLmaxL;
            float Q = Mathf.Exp((1 - C / (2 * L)) * (HCL.HCLgamma / HCL.HCLy0));
            float U = (2 * L - C) / (2 * Q - 1);
            float V = C / Q;
            float A = (H + Mathf.Min(MathIsHard.Frac(2 * H) / 4, MathIsHard.Frac(-2 * H) / 8)) * Mathf.PI * 2;
            float T;
            H *= 6;
            if (H <= 0.999)
            {
                T = Mathf.Tan(A);
                RGB.x = 1;
                RGB.y = T / (1 + T);
            }
            else if (H <= 1.001)
            {
                RGB.x = 1;
                RGB.y = 1;
            }
            else if (H <= 2)
            {
                T = Mathf.Tan(A);
                RGB.x = (1 + T) / T;
                RGB.y = 1;
            }
            else if (H <= 3)
            {
                T = Mathf.Tan(A);
                RGB.y = 1;
                RGB.z = 1 + T;
            }
            else if (H <= 3.999)
            {
                T = Mathf.Tan(A);
                RGB.y = 1 / (1 + T);
                RGB.z = 1;
            }
            else if (H <= 4.001)
            {
                RGB.y = 0;
                RGB.z = 1;
            }
            else if (H <= 5)
            {
                T = Mathf.Tan(A);
                RGB.x = -1 / T;
                RGB.z = 1;
            }
            else
            {
                T = Mathf.Tan(A);
                RGB.x = 1;
                RGB.z = -T;
            }

            RGB *= V;
            RGB.x += U;
            RGB.y += U;
            RGB.z += U;
        }
        return new Color(RGB.x, RGB.y, RGB.z, hCL.alpha);
    }
}
