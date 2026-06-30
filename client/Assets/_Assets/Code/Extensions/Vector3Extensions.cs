using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 RotateAround(this Vector3 position, Vector3 point, Vector3 axis, float angle)
    {
        Vector3 vector = position;
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);
        Vector3 vector2 = vector - point;
        vector2 = rotation * vector2;
        vector = point + vector2;
        return vector;
    }
}
