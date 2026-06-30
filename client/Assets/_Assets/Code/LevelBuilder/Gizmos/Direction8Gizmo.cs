using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Direction8Gizmo : Gizmo
{
    public Direction8 direction{
        get{
            if(!levelEntity.GetSerializableData().gizmoIndex.HasValue)
                levelEntity.GetSerializableData().gizmoIndex = (int) GetFirstAvailableDirection();

            return (Direction8) levelEntity.GetSerializableData().gizmoIndex.Value;
        }
        set{
            levelEntity.GetSerializableData().gizmoIndex = (int) value;
        }
    }

    public bool up = true;
    public bool right_up = true;
    public bool right = true;
    public bool right_down = true;
    public bool down = true;
    public bool left_down = true;
    public bool left = true;
    public bool left_up = true;

    public Transform face;
    public Transform shadow;

    private void Start()
    {
        face.eulerAngles = new Vector3(0,0, direction.ToDegree());
        shadow.eulerAngles = new Vector3(0,0, direction.ToDegree());
    }

    public override void On_TouchStart(TouchInfo touch)
    {
        if (touch.pickedUIElement != null)
            return;

        if (touch.GetFirstPickedGameObject(Camera.main) != gameObject)
			return;

        base.On_TouchStart(touch);

		assignedFingerIndex = touch.fingerIndex;
        
        touchOffset = transform.position - touch.GetTouchToWorldPoint(10, Camera.main);
    }

    public override void On_TouchMaintained(TouchInfo touch)
    {
        if (touch.fingerIndex != assignedFingerIndex)
			return;

        base.On_TouchMaintained(touch);

        Vector3 touchPos = touch.GetTouchToWorldPoint(10, Camera.main) + (Vector3)touchOffset;
        levelEntity.SetPositon(touchPos);
    }  

    public override void On_TouchEnd(TouchInfo touch)
    {
        if (touch.fingerIndex != assignedFingerIndex)
			return;

        base.On_TouchEnd(touch);

        assignedFingerIndex = -1;
        
        levelEntity.OnFinishedMoving();
    }

    public override void Tap()
    {
        face.transform.DOComplete();
        shadow.transform.DOComplete();

		face.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 1);
		shadow.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 1);

		int z = ChangeDirection();

		face.transform.DORotate(new Vector3(0f, 0f, -z), 0.3f, RotateMode.FastBeyond360).SetRelative(true);
        shadow.transform.DORotate(new Vector3(0f, 0f, -z), 0.3f, RotateMode.FastBeyond360).SetRelative(true);
    }

    public float GetZRotation(Direction8 direction)
	{
		switch(direction)
		{
			default:
			case Direction8.Up:
				return 0f;
            case Direction8.Right_Up:
                return 45f;
            case Direction8.Right:
				return 90f;
            case Direction8.Right_Down:
                return 135f;
            case Direction8.Down:
				return 180f;
            case Direction8.Left_Down:
                return 225f;
            case Direction8.Left:
				return 270f;
            case Direction8.Left_Up:
                return 315f;
        }
	}

    public Direction8 GetFirstAvailableDirection()
    {
        if(up)
            return Direction8.Up;
        if (right_up)
            return Direction8.Right_Up;
        if (right)
            return Direction8.Right;
        if (right_down)
            return Direction8.Right_Down;
        if (down)
            return Direction8.Down;
        if (left_down)
            return Direction8.Left_Down;
        if (left)
            return Direction8.Left;
        if (left_up)
            return Direction8.Left_Up;

        Debug.LogError("No available directions!");
        return Direction8.Up;
    }

    public int ChangeDirection(int loopCounter = 0)
	{
		if(loopCounter > 3)
		{
			Debug.LogError("No valid rotation. Inf loop stopped!");
			return 0;
		}

		int enumInt = (int) direction;
		enumInt++;

		if(enumInt > 7)
			enumInt = 0;

		direction = (Direction8)enumInt;
		
		if(	(direction == Direction8.Up && !up) ||
			(direction == Direction8.Left && !left) ||
			(direction == Direction8.Down && !down) ||
			(direction == Direction8.Right && !right) ||
            (direction == Direction8.Right_Up && !right_up) ||
            (direction == Direction8.Right_Down && !right_down) ||
            (direction == Direction8.Left_Up && !left_up) ||
            (direction == Direction8.Left_Down && !left_down))
        {
				return ChangeDirection(loopCounter + 1);
			}

		return 45 * (1 + loopCounter);
	}

    public override void SetColor(Color color)
    {
    }
    
    private void OnEnable()
    {
        face.transform.eulerAngles = new Vector3(0f, 0f, -GetZRotation(direction));
        shadow.transform.eulerAngles = face.transform.eulerAngles;
        
        TouchInput.On_TouchStart += On_TouchStart;
        TouchInput.On_TouchMaintained += On_TouchMaintained;
        TouchInput.On_TouchEnd += On_TouchEnd;
    }

    private void Unsubscribe()
    {
        TouchInput.On_TouchStart -= On_TouchStart;
        TouchInput.On_TouchMaintained -= On_TouchMaintained;
        TouchInput.On_TouchEnd -= On_TouchEnd;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    protected override void OnDestroy()
    {
		base.OnDestroy();
        Unsubscribe();
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		face.transform.DOKill();
		shadow.transform.DOKill();
	}
}
