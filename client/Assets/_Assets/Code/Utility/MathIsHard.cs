using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathIsHard {

	/// <summary>
	/// Normalize any float
	/// </summary>
	/// <param name="value">The Value to normalize</param>
	/// <param name="min">minimum value</param>
	/// <param name="max">maximum value</param>
	/// <returns></returns>
	public static float Normalize(float value, float min, float max)
	{
		float range = max - min;
		int overflowCount = Mathf.FloorToInt(value / range);
		float t = (value - min) / (max - min);
		float tNormalized;

		if(overflowCount < 0)
			tNormalized = Mathf.Abs(overflowCount) - t;
		else
			tNormalized = t - overflowCount;

		return Mathf.Lerp(min, max, tNormalized);
	}

	/// <summary>
	/// Useful when translating from vector direction to z-angle in 2D.
	/// </summary>
	public static float DirectionToEuler(Vector2 direction)
	{
		float value = (float)((Mathf.Atan2(direction.x, direction.y) / Mathf.PI) * 180f);
		if(value < 0) value += 360f;
	
		return value;
	}

	/// <summary>
	/// Useful when translating from z-angle to a vector direction.
	/// </summary>
	/// <param name="degree"></param>
	/// <returns></returns>
	public static Vector2 EulerToDirection(float degree)
	{
		return new Vector2( Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad) );
	}

	/// <summary>
	/// Is value larger or equal to min and less or equal to max?
	/// </summary>
	public static bool IsWithinRange(float value, float min, float max)
	{
		return value >= min && value <= max;
	}

	/// <summary>
	/// Returns difference between two values
	/// </summary>
	public static float DistanceBetweenFloats(float a, float b)
    {
        return Mathf.Abs(a - b);
    }

	/// <summary>
	/// Finds the position of where two imaginery lines would cross each other, if the lines never cross returns vector.zero
	/// </summary>
	/// <param name="rayOrigin1">start of first ray</param>
	/// <param name="rayDirection1">direction of first ray</param>
	/// <param name="rayOrigin2">start of second ray</param>
	/// <param name="rayDirection2">direction of second ray</param>
	/// <returns></returns>
	public static Vector2 GetLinesIntersectionPoint(Vector2 rayOrigin1, Vector2 rayDirection1, Vector2 rayOrigin2, Vector2 rayDirection2)
	{
		rayDirection1 = rayDirection1.normalized * int.MaxValue;
		rayDirection2 = rayDirection2.normalized * int.MaxValue;

		Vector3 rayDirection3 = rayOrigin2 - rayOrigin1;
		Vector3 crossVec1and2 = Vector3.Cross(rayDirection1, rayDirection2);
		Vector3 crossVec3and2 = Vector3.Cross(rayDirection3, rayDirection2);
 
		float planarFactor = Vector3.Dot(rayDirection3, crossVec1and2);
 
		//is coplanar, and not parrallel
		if(Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
		{
			float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
			return rayOrigin1 + (rayDirection1 * s);
		}
		else
		{
			return Vector3.zero;
		}
	}

	/// <summary>
	/// Also called binormal of the surface. equal to normal +/- 90 degress.
	/// </summary>
	/// <param name="hitNormal"></param>
	/// <returns></returns>
	public static Vector2 GetSurfaceDirectionFromSurfaceNormal(Vector2 hitNormal)
	{
		return Quaternion.AngleAxis(-90, Vector3.forward) * hitNormal;
	}

	public static float Frac(float value)
	{
		//In C# casting a float to int truncates the value.
		return value - (int)value;
	}

	public static float Round(float value, int digits)
	{
		float mult = Mathf.Pow(10.0f, (float)digits);
		return Mathf.Round(value * mult) / mult;
	}
}
