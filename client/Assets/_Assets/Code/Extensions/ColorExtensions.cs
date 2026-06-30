using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions
{
    public static Color SetAlpha(this Color color, float alpha)
    {
        Color result = new Color(color.r, color.g, color.b, alpha);
        color = result;
        return color;
    }


    public static float GetVibrance(this Color color)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        return v;
    }

    public static float GetSaturation(this Color color)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        return s;
    }

    public static float GetHue(this Color color)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        return h;
    }

    public static Color SetVibrance(this Color color, float vibrance)
    {
        float h, s, v;

        float alpha = color.a;
        Color.RGBToHSV(color, out h, out s, out v);

        v = ((Mathf.Clamp01(vibrance)*255f)/255f);

        color = Color.HSVToRGB(h, s, v);
        color.a = alpha;
        return color;
    }

    public static Color SetSaturation(this Color color, float saturation)
    {
        float h, s, v;

        float alpha = color.a;
        Color.RGBToHSV(color, out h, out s, out v);

        s = ((Mathf.Clamp01(saturation)*255f)/255f);
        
        color = Color.HSVToRGB(h, s, v);
        color.a = alpha;
        return color;
    }

	public static Vector3Int ToVector3Int(this Color color)
	{
		return new Vector3Int(Mathf.RoundToInt(color.r * 255), Mathf.RoundToInt(color.g * 255), Mathf.RoundToInt(color.b * 255));
	}
}
