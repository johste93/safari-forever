using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleGizmo : Gizmo
{
    public BoxCollider2D boxCollider2D;
    public SpriteRenderer sprite;
    public SpriteRenderer shadow;

    public Direction4 scaleDirection = Direction4.Up;
    private float minimumDistance = 1f;

    public float GetLength() //Use this as a relative value.
    {
        if(levelEntity == null)
            return 0;

        return Vector2.Distance(levelEntity.GetCenterPosition(), transform.position);
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

        if(LevelEntity.On_EntityStartedChangingSize != null)
            LevelEntity.On_EntityStartedChangingSize(levelEntity);
    }

    public override void On_TouchMaintained(TouchInfo touch)
    {
        if (touch.fingerIndex != assignedFingerIndex)
			return;

        base.On_TouchMaintained(touch);

        Vector3 touchPos = touch.GetTouchToWorldPoint(10, Camera.main) + (Vector3)touchOffset;
        Vector3 targetPos = transform.parent.InverseTransformPoint(touchPos);
        Vector2 target = Vector2.zero;
        Vector2 center = levelEntity.GetCenterPosition();

        switch(scaleDirection)
        {
           case Direction4.Up:
                target = new Vector2(center.x, targetPos.y);
                target.y = (int)target.y;
                target.y = Mathf.Max(center.y + minimumDistance, target.y);
                
           break;
           case Direction4.Right:
                target = new Vector2(targetPos.x, center.y);
                target.x = (int)target.x;
                target.x = Mathf.Max(center.x + minimumDistance, target.x);
           break;
           case Direction4.Down:
                target = new Vector2(center.x, targetPos.y);
                target.y = (int)target.y;
                target.y = Mathf.Min(center.y - minimumDistance, target.y);
           break;
           case Direction4.Left:
                target = new Vector2(targetPos.x, center.y);
                target.x = (int)target.x;
                target.x = Mathf.Min(center.x - minimumDistance, target.x);
           break;
        }

        

        if((Vector2)transform.localPosition != target)
        {
            transform.localPosition = target;
            levelEntity.UpdateSize();
            //Audio.Play( SFX.instance.ui.levelEntityScaling, Channel.UI ).SetPitch(Random.Range(0.9f, 1f)).SetVolume(0.25f);
        } 
    }  

    public override void On_TouchEnd(TouchInfo gesture)
    {
        if (gesture.fingerIndex != assignedFingerIndex)
			return;

        base.On_TouchEnd(gesture);

        levelEntity.UpdateScaleGizmoPositions();

		assignedFingerIndex = -1;

        if(LevelEntity.On_EntityStoppedChangingSize != null)
            LevelEntity.On_EntityStoppedChangingSize(levelEntity);

    }

    public override void SetColor(Color color)
    {
        sprite.color = color;
    }

    public void SetWidth(float width)
    {
        //boxCollider2D.size = new Vector2(width, boxCollider2D.size.y);
    }

    private void OnEnable()
    {
        gameObject.name = $"ScaleGizmo ({scaleDirection})";

        TouchInput.On_TouchStart += On_TouchStart;
        TouchInput.On_TouchMaintained += On_TouchMaintained;
        TouchInput.On_TouchEnd += On_TouchEnd;

        switch(scaleDirection)
        {
            case Direction4.Up:
                shadow.transform.localPosition = new Vector2(0f, 0.25f);
            break;
            case Direction4.Right:
                shadow.transform.localPosition = new Vector2(0.1f, 0.35f);
            break;
            case Direction4.Down:
                shadow.transform.localPosition = new Vector2(0f, 0.45f);
            break;
            case Direction4.Left:
                shadow.transform.localPosition = new Vector2(-0.1f, 0.35f);
            break;
        }
        
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
