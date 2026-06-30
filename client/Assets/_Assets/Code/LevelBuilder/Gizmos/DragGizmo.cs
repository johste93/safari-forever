using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragGizmo : Gizmo
{   
    public SpriteRenderer sprite;

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

    public override void SetColor(Color color)
    {
        if(sprite != null)
            sprite.color = color;
    }

    private void OnEnable()
    {
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
    }
}
