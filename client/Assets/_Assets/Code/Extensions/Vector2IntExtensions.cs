using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2IntExtensions
{
    public static Vector2 ToVector(this Vector2Int vector)
    {
        return new Vector2(vector.x, vector.y);
    }
}
