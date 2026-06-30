using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public static class Texture2DExtensions
{
    public static Sprite ToSprite(this Texture2D texture2D)
    {
        return Sprite.Create(texture2D, new Rect(0,0,texture2D.width, texture2D.height), Vector2.one * 0.5f);
    }

    public static byte[] EncodeToGIF (this Texture2D texture2D)
    {
        return GIFConverter.EncodeToGIF(texture2D);
    }
}
