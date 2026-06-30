using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Direction4Gizmo : Gizmo
{
    public Direction4 direction{
        get{
            if(!levelEntity.GetSerializableData().gizmoDirection.HasValue)
                levelEntity.GetSerializableData().gizmoDirection = GetFirstAvailableDirection();

            return levelEntity.GetSerializableData().gizmoDirection.Value;
        }
        set{
            levelEntity.GetSerializableData().gizmoDirection = value;
        }
    }

    public bool up = true;
    public bool right = true;
    public bool down = true;
    public bool left = true;

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

    public float GetZRotation(Direction4 direction)
	{
		switch(direction)
		{
			default:
			case Direction4.Up:
				return 0f;
			case Direction4.Right:
				return 90f;
			case Direction4.Down:
				return 180f;
			case Direction4.Left:
				return 270f;
		}
	}

    public Direction4 GetFirstAvailableDirection()
    {
        if(up)
            return Direction4.Up;
        if(right)
            return Direction4.Right;
        if(down)
            return Direction4.Down;
        if(left)
            return Direction4.Left;

        Debug.LogError("No available directions!");
        return Direction4.Up;
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

		if(enumInt > 3)
			enumInt = 0;

		direction = (Direction4)enumInt;
		
		if(	(direction == Direction4.Up && !up) ||
			(direction == Direction4.Left && !left) ||
			(direction == Direction4.Down && !down) ||
			(direction == Direction4.Right && !right))
			{
				return ChangeDirection(loopCounter + 1);
			}

		return 90 * (1 + loopCounter);
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
