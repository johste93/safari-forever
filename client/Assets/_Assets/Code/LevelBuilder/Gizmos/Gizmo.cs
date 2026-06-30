using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Gizmo : MonoBehaviour
{
    public LevelEntity levelEntity;
    public int priority = 0;
    public Transform graphic;

    protected int assignedFingerIndex = -1;
    protected Vector2 touchOffset;

    public delegate void TouchEvent(TouchInfo gesture);
    public TouchEvent OnTouchStart;
    public TouchEvent OnTouchMaintained;
    public TouchEvent OnTouchEnd;

    public delegate void TapEvent();
    public TapEvent On_Tap;

    public bool includeInPositionsInside = true;

    public BoxCollider2D selectCollider2D;

    public virtual void Initalize(LevelEntity parentEntity)
    {
        levelEntity = parentEntity;
    }

    public void AssignFinger(int fingerIndex)
    {
        assignedFingerIndex = fingerIndex;
    }

    public virtual void On_TouchStart(TouchInfo touch)
    {
        if (touch.pickedUIElement != null)
            return;

        graphic.DOComplete();
        graphic.DOPunchScale(Vector3.one * 1.3f, 0.4f, 1);

        if(OnTouchStart != null)
            OnTouchStart(touch);
    }

    public virtual void On_TouchMaintained(TouchInfo touch)
    {
        if (OnTouchMaintained != null)
            OnTouchMaintained(touch);
    }  

    public virtual void On_TouchEnd(TouchInfo touch)
    {
        if (OnTouchEnd != null)
            OnTouchEnd(touch);

        if(touch.duration < Globals.gameConstants.standardTapDuration)
        {
            Tap();
        }
    }

    public virtual void Tap()
    {
        On_Tap?.Invoke();
    }

    public virtual void SetColor(Color color)
    {
    }

    public void Scale(Vector2 size)
    {
        if(selectCollider2D == null)
            return;

        selectCollider2D.size = size;
    }

	protected virtual void OnDestroy()
	{
		graphic.DOKill();
	}
}