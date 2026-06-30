using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{
    public static Direction4 ToDirection(this Vector2 vector)
    {
        vector = vector.RoundVector2ToInt();

        if(vector == Vector2.up)
            return Direction4.Up;

        if(vector == Vector2.right)
            return Direction4.Right;

        if(vector == Vector2.down)
            return Direction4.Down;

        if(vector == Vector2.left)
            return Direction4.Left;

        Debug.LogError("Not able to map vector to direction!");
        return Direction4.Up;
    }

    public static Vector2Int RoundVector2ToInt(this Vector2 vector)
    {
        return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
    }

    public static Vector2Int FloorVector2ToInt(this Vector2 vector)
    {
        return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
    }

    public static Vector2Int CeilVector2ToInt(this Vector2 vector)
    {
        return new Vector2Int(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
    }

    public static Vector2 Rotate(this Vector2 v, float degrees) 
    {
         float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
         float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
         float tx = v.x;
         float ty = v.y;
         v.x = (cos * tx) - (sin * ty);
         v.y = (sin * tx) + (cos * ty);
         return v;
    }

    public static float ToAngle(this Vector2 v)
    {
        if (v.x < 0)
        {
            return 360 - (Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
        }
    }
}
