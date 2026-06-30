using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GifQualitySettings
{
    public GifQuality quality;
    public float duration = 15f;
    public int fps = 15;
    public int width = 400;
    public int height = 500;
    public int processingQuality = 5;

    public override string ToString()
    {
        return $"{quality}\nduration: {duration}\nfps: {fps}\nresolution: {width}x{height}\nquality: {processingQuality}";
    }
}
