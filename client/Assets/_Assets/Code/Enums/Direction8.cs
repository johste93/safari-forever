using UnityEngine;

public enum Direction8 {
	Up,
    Right_Up,
	Right,
    Right_Down,
	Down,
    Left_Down,
	Left,
    Left_Up
}

static class Direction8Functions
{	
	public static Direction8 Mirror(this Direction8 direction)
	{
		switch (direction)
		{
			default:
			case Direction8.Down:
				return Direction8.Up;
			case Direction8.Up:
				return Direction8.Down;
			case Direction8.Left:
				return Direction8.Right;
			case Direction8.Right:
				return Direction8.Left;
            case Direction8.Right_Up:
                return Direction8.Left_Down;
            case Direction8.Right_Down:
                return Direction8.Left_Up;
            case Direction8.Left_Up:
                return Direction8.Right_Down;
            case Direction8.Left_Down:
                return Direction8.Right_Up;
        }
	}

	public static int ToDegree(this Direction8 direction)
	{
		switch (direction)
		{
			default:
			case Direction8.Up:
				return 0;
            case Direction8.Left_Up:
                return 45;
            case Direction8.Left:
				return 90;
            case Direction8.Left_Down:
                return 135;
            case Direction8.Down:
				return 180;
            case Direction8.Right_Down:
                return 225;
            case Direction8.Right:
				return 270;
            case Direction8.Right_Up:
                return 315;
        }
	} 

	public static Vector2 ToVector(this Direction8 direction)
	{
		switch (direction)
		{
			default:
			case Direction8.Up:
				return Vector2.up;
			case Direction8.Right:
				return Vector2.right;
			case Direction8.Down:
				return Vector2.down;
			case Direction8.Left:
				return Vector2.left;
            case Direction8.Right_Up:
                return Vector2.one;
            case Direction8.Right_Down:
                return new Vector2(1,-1);
            case Direction8.Left_Up:
                return new Vector2(-1, 1);
            case Direction8.Left_Down:
                return -Vector2.one;
        }
	}

	public static bool IsHorizontal(this Direction8 direction)
	{
		switch (direction)
		{
			default:
			case Direction8.Up:
			case Direction8.Down:
            case Direction8.Left_Down:
            case Direction8.Left_Up:
            case Direction8.Right_Down:
            case Direction8.Right_Up:
				return false;
			case Direction8.Right:
			case Direction8.Left:
				return true;
		}
	}

	public static bool IsVertical(this Direction8 direction)
	{
		switch (direction)
		{
			default:
			case Direction8.Up:
			case Direction8.Down:
				return true;
			case Direction8.Right:
			case Direction8.Left:
            case Direction8.Left_Down:
            case Direction8.Left_Up:
            case Direction8.Right_Down:
            case Direction8.Right_Up:
                return false;
		}
	}

    public static bool IsDiagonal(this Direction8 direction)
    {
        switch (direction)
        {
            default:
            case Direction8.Up:
            case Direction8.Down:
            case Direction8.Right:
            case Direction8.Left:
                return false;
            case Direction8.Left_Down:
            case Direction8.Left_Up:
            case Direction8.Right_Down:
            case Direction8.Right_Up:
                return true;
        }
    }
}
