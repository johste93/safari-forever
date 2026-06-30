using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{
    public static float Distance(this float f, float other)
    {
        return Mathf.Abs(f - other);
    }   
}
