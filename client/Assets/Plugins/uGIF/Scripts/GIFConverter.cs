using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uGIF;
using System.IO;

public class GIFConverter
{
    public static byte[] EncodeToGIF(Texture2D texture2D)
    {
        Image img = new Image(texture2D);
        return Encode(img);
    }

    public static byte[] Encode (Image img32)
    {
        var ge = new GIFEncoder ();
        ge.useGlobalColorTable = false;
        ge.repeat = 0;
        ge.FPS = 1;
        ge.transparent = null;
        ge.dispose = 1;

        using(MemoryStream stream = new MemoryStream ())
        {
            ge.Start (stream);
        
            img32.Flip ();
            ge.AddFrame (img32);

            ge.Finish ();
            return stream.ToArray ();
        }
    }
}
