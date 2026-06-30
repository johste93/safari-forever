using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3IntExtensions
{
    public static Vector3Int RoundVector3ToInt(this Vector3 vector)
    {
        return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
    }

    public static Vector3Int FloorVector3ToInt(this Vector3 vector)
    {
        return new Vector3Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
    }

    public static Vector3Int CeilVector3ToInt(this Vector3 vector)
    {
        return new Vector3Int(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y), Mathf.CeilToInt(vector.z));
    }
}
