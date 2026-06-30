using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotationGizmo : Gizmo
{
    public Rotation rotation{
        get{
            if(!levelEntity.GetSerializableData().gizmoRotation.HasValue)
                levelEntity.GetSerializableData().gizmoRotation = Rotation.Clockwise;

            return levelEntity.GetSerializableData().gizmoRotation.Value;
        }
        set{
            levelEntity.GetSerializableData().gizmoRotation = value;
        }
    }

    public Transform arrows;

    public SpriteRenderer face;
    public Transform shadow;

    private void Start()
    {
        int intRotation = (int) rotation;
        arrows.localScale = new Vector3(0.7f * intRotation, 0.7f, 1f);
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

		face.transform.DORotate(new Vector3(0f, 0f, 0f), 0.3f, RotateMode.FastBeyond360).SetRelative(true);
        shadow.transform.DORotate(new Vector3(0f, 0f, 0f), 0.3f, RotateMode.FastBeyond360).SetRelative(true);

        ChangeDirection();
    } 

    private void ChangeDirection()
    {
        int intRotation = (int) rotation;
        intRotation *= -1;
        rotation = (Rotation) intRotation;

        arrows.localScale = new Vector3(0.7f * intRotation, 0.7f, 1f);
        arrows.DOComplete();
        arrows.DORotate(new Vector3(0f, 0f, 360f * -intRotation), 1f, RotateMode.FastBeyond360).SetEase(Ease.OutSine).SetRelative(true);
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
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		face.transform.DOKill();
        shadow.transform.DOKill();
		arrows.DOKill();
	}
}
