using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position2D
{
    public float x;
    public float y;

    public Position2D()
    {
    }

    public Position2D(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Position2D(Vector2 vector2)
    {
        this.x = vector2.x;
        this.y = vector2.y;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}
