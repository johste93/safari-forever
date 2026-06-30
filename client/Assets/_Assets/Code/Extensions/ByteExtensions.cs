using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using GifPlayer;

public static class ByteExtensions
{
    public static string ToUTF8(this byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }

    public static string ToBase64(this byte[] bytes)
    {
        return Convert.ToBase64String(bytes);
    }

	public static Sprite ToSpriteFromPNG(this byte[] bytes)
    {
        Texture2D tex = new Texture2D(1,1);
		tex.LoadImage(bytes);
		return tex.ToSprite();
    }

	public static Sprite ToSpriteFromGIF(this byte[] bytes)
    {
		return GifProtocol.ToSprite(bytes);
    }
}
