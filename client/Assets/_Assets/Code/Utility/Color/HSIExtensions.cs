using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HSIExtensions
{
    public static Color ToRGB(this HSI hSI)
    {
		Color c = new Color();
       	float x = hSI.intensity * (1 - hSI.saturation);		
		if(hSI.hue < 2 * Mathf.PI / 3)
		{
			float y = hSI.intensity * (1 + (hSI.saturation * Mathf.Cos(hSI.hue)) / (Mathf.Cos(Mathf.PI / 3 - hSI.hue)));
			float z = 3 * hSI.intensity - (x + y);
			c.b = x; c.r = y; c.g = z;
		}
		else if(hSI.hue < 4 * Mathf.PI / 3)
		{
			float y = hSI.intensity * (1 + (hSI.saturation * Mathf.Cos(hSI.hue - 2 * Mathf.PI / 3)) / (Mathf.Cos(Mathf.PI / 3 - (hSI.hue  - 2 * Mathf.PI / 3))));
			float z = 3 * hSI.intensity - (x + y);
			c.r = x; c.g = y; c.b = z;
		}
		else
		{
			float y = hSI.intensity * (1 + (hSI.saturation * Mathf.Cos(hSI.hue - 4 * Mathf.PI / 3)) / (Mathf.Cos(Mathf.PI / 3 - (hSI.hue  - 4 * Mathf.PI / 3))));
			float z = 3 * hSI.intensity - (x + y);
			c.r = z; c.g = x; c.b = y;
		}

		return c;
    }

    public static HSI ToHSI(this Color color)
    {
        float hue, saturation, intensity;
		
		intensity = (color.r + color.g + color.b) / 3f;
		
		if (color.r == color.g && color.g == color.b){
			saturation = hue = 0;
		}
		else 
        {
			float w = (color.r - color.g + color.r - color.b) / Mathf.Sqrt((color.r - color.g) * (color.r - color.g) + (color.r - color.b) * (color.g - color.b)) / 2;
			hue = Mathf.Acos(w) * 180 / Mathf.PI;

			if (color.b > color.g)
				hue = 360 - hue;

			saturation = 1 - Mathf.Min(color.r, color.g, color.b) / intensity;
		}

		return new HSI(hue,saturation,intensity,color.a);
    }
}
