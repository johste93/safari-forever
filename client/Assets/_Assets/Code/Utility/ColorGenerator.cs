using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{   

    public static float[] vibranceFallOff = new float[]{
        0.10f, //.90f
        0.25f, //0.75f
        0.60f, //0.60f
        0.70f //0.30f
    };

    public static ColorPalette GetRandomColorPalette(bool scramble = false)
	{
		Color[] colors = new Color[4];
        
        //Debug.Log(Globals.gameConstants.colorSpace);
        switch(Globals.gameConstants.colorSpace)
        {
            case ColorSpace.RGB:
                for(int n = 0; n < 4; n++)
                {
                    colors[n] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                }
            break;
            case ColorSpace.HSV:
                float vibrance = 1f;
                float saturation = Random.Range(0.3f,0.9f);
                
                
                for(int n = 0; n < 4; n++)
                {
                    float hue = Random.Range(0f,1f);
                    colors[n] = Color.HSVToRGB(hue, saturation, vibrance - vibranceFallOff[n]);
                }
            break;
            case ColorSpace.HCL:

                float lightness = Random.Range(0.5f,1f);
                float chroma = Random.Range(0.3f,1f);
                float hueStart = Random.Range(0f, 1f);
                
                for(int n = 0; n < 4; n++)
                {
                    float hue = MathIsHard.Normalize((hueStart + (n*0.25f)), 0f, 1f);
                    HCL hcl = new HCL(hue, chroma, lightness, 1f);
                    colors[n] = hcl.ToRGB();
                }
            break;
            case ColorSpace.HSI:
                
                float s = Random.Range(0.3f,1f);
                float i = 1f;
                
                for(int n = 0; n < 4; n++)
                {
                    float hue = Random.Range(0f,1f);
                    HSI hsi = new HSI(hue, s, i, 1f);
                    colors[n] = hsi.ToRGB();
                }
            break;
        }

		
        ColorPalette result = (ColorPalette) ScriptableObject.CreateInstance(typeof(ColorPalette));

        result.main     = colors[0];
        result.floor    = colors[1];
        result.wall     = colors[2];
        result.pattern  = colors[3];
        
        

        if(scramble)
            result = Scramble(result);

        return result;
	}

    public static ColorPalette GetHighContrastMonoChromatic()
    {
        Color[] colors = new Color[4];

        float hue = Random.Range(0f, 1f);
        float lightness = 0f;
        float chroma = 0f;

        for(int i = 0; i < 4; i++)
        {
            hue = Random.Range(0f, 1f);
            lightness += 0.2f;
            HCL hcl = new HCL(hue, chroma, lightness, 1f);
            colors[i] = hcl.ToRGB();
        }

        ColorPalette result = (ColorPalette) ScriptableObject.CreateInstance(typeof(ColorPalette));

        result.main     = colors[0];
        result.floor    = colors[1];
        result.wall     = colors[2];
        result.pattern  = colors[3];

        return result;
    }

    public static Color GetRandomColor(float vibrance = 1f)
    {
        float saturation = Random.Range(0f,1f);
        float hue = Random.Range(0f,1f);
        return Color.HSVToRGB(hue, saturation, vibrance);
    }

    private static ColorPalette Scramble(ColorPalette input)
    {
        ColorPalette result = (ColorPalette) ScriptableObject.CreateInstance(typeof(ColorPalette));
        List<Color> colors = new List<Color>{
            input.main,
            input.floor,
            input.pattern,
            input.wall
        };

        int randomIndex = Random.Range(0, colors.Count);
        result.main = colors[randomIndex];
        colors.RemoveAt(randomIndex);

        randomIndex = Random.Range(0, colors.Count);
        result.floor = colors[randomIndex];
        colors.RemoveAt(randomIndex);

        randomIndex = Random.Range(0, colors.Count);
        result.pattern = colors[randomIndex];//.SetAlpha(0.5f);
        colors.RemoveAt(randomIndex);

        randomIndex = Random.Range(0, colors.Count);
        result.wall = colors[randomIndex];
        colors.RemoveAt(randomIndex);

        return result;
    }
}
