using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCorner : MonoBehaviour
{
    public SpriteRenderer[] triangles;

    public float defaultRotation;

    public Corner corner;

    public void SetColor(Color a, Color b)
    {
        triangles[0].color = a;
        triangles[1].color = b;
    }

    public void SetEnabled(bool enabled)
    {
        foreach(SpriteRenderer sR in triangles)
        {
            sR.enabled = enabled;
        }
    }

    public void FlippRotation(bool flipped)
    {
        transform.eulerAngles = new Vector3(0,0, flipped ? defaultRotation : defaultRotation+180 );
    }

    public enum Corner
    {
        topLeft,
        topRight,
        bottomLeft,
        bottomRight
    }
}


