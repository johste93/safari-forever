using UnityEngine;

public enum Direction4 {
	Up,
	Right,
	Down,
	Left
}

static class Direction4Functions
{	
	public static Direction4 Mirror(this Direction4 direction)
	{
		switch (direction)
		{
			default:
			case Direction4.Down:
				return Direction4.Up;
			case Direction4.Up:
				return Direction4.Down;
			case Direction4.Left:
				return Direction4.Right;
			case Direction4.Right:
				return Direction4.Left;
		}
	}

	public static int ToDegree(this Direction4 direction)
	{
		switch (direction)
		{
			default:
			case Direction4.Up:
				return 0;
			case Direction4.Left:
				return 90;
			case Direction4.Down:
				return 180;
			case Direction4.Right:
				return 270;
		}
	} 

	public static Vector2 ToVector(this Direction4 direction)
	{
		switch (direction)
		{
			default:
			case Direction4.Up:
				return Vector2.up;
			case Direction4.Right:
				return Vector2.right;
			case Direction4.Down:
				return Vector2.down;
			case Direction4.Left:
				return Vector2.left;
		}
	}

	public static bool IsHorizontal(this Direction4 direction)
	{
		switch (direction)
		{
			default:
			case Direction4.Up:
			case Direction4.Down:
				return false;
			case Direction4.Right:
			case Direction4.Left:
				return true;
		}
	}

	public static bool IsVertical(this Direction4 direction)
	{
		switch (direction)
		{
			default:
			case Direction4.Up:
			case Direction4.Down:
				return true;
			case Direction4.Right:
			case Direction4.Left:
				return false;
		}
	}
}
