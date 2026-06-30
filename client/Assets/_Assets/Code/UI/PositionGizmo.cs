using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionGizmo : Gizmo
{
    public Vector2 position{
        get{
            if(parentGizmo.levelEntity.GetSerializableData().gizmoOffset == null)
                parentGizmo.levelEntity.GetSerializableData().gizmoOffset = new Position2D(2,0);

            return parentGizmo.levelEntity.GetSerializableData().gizmoOffset.ToVector2();
        }
        set{
            parentGizmo.levelEntity.GetSerializableData().gizmoOffset = new Position2D(value.x, value.y);
        }
    }

    public SpriteRenderer sprite;
    public Gizmo parentGizmo;

    public delegate void PositionGizmoEvent();
    public PositionGizmoEvent On_PositionMoved;
     
    private void Awake()
    {
        Initalize(parentGizmo.levelEntity);

        //Debug.Log(levelEntity);
        SetColor(levelEntity.GetColor());
    }

    private void Start()
    {
        transform.position = levelEntity.gizmo.transform.position + (Vector3) position;
    }

    public override void On_TouchStart(TouchInfo touch)
    {
        if (touch.pickedUIElement != null)
            return;
            
        if(touch.GetFirstPickedGameObject(Camera.main) != gameObject)
			return;

        base.On_TouchStart(touch);

		assignedFingerIndex = touch.fingerIndex;
        
        touchOffset = transform.position - touch.GetTouchToWorldPoint(10, Camera.main);
    }

    public override void On_TouchMaintained(TouchInfo touch)
    {
        if(touch.fingerIndex != assignedFingerIndex)
			return;

        base.On_TouchMaintained(touch);

       
        Vector2 touchPos = touch.GetTouchToWorldPoint(10, Camera.main) + (Vector3)touchOffset;
        Vector2 snappedPosition = touchPos.FloorVector2ToInt() + Globals.gameConstants.tileOffset;
        Vector2 newOffset = snappedPosition - (Vector2)levelEntity.gizmo.transform.position;

        if(position != newOffset && newOffset != Vector2.zero)
        {
            Vector3 worldPos = levelEntity.gizmo.transform.position + (Vector3)newOffset;
            worldPos = ClampPositionToInsideRect(worldPos);
            newOffset = worldPos - levelEntity.gizmo.transform.position;
            if (newOffset == Vector2.zero)
                return;

            position = newOffset;
            transform.position = worldPos;
            if (On_PositionMoved != null)
                On_PositionMoved();
        }
    }

    private Vector2 ClampPositionToInsideRect(Vector2 pos)
    {
        Vector2 result = pos;
        Vector2 halfSize = new Vector2(Globals.gameConstants.levelWidth, Globals.gameConstants.levelHeight) / 2f;
        if (pos.x < -halfSize.x)
            result.x = -(halfSize.x - Globals.gameConstants.tileOffset.x);

        if (pos.x > halfSize.x)
            result.x = (halfSize.x - Globals.gameConstants.tileOffset.x);

        if (pos.y < -halfSize.y)
            result.y = -(halfSize.y - Globals.gameConstants.tileOffset.y);

        if (pos.y > halfSize.y)
            result.y = (halfSize.y - Globals.gameConstants.tileOffset.y);

        return result;
    }

    private bool PositionInsideLevelRect(Vector2 pos)
    {
        Vector2 halfSize = new Vector2(Globals.gameConstants.levelWidth, Globals.gameConstants.levelHeight) / 2f;
        return !(pos.x < -halfSize.x || pos.x > halfSize.x || pos.y < -halfSize.y || pos.y > halfSize.y);
    }

    public override void On_TouchEnd(TouchInfo touch)
    {
        if(touch.fingerIndex != assignedFingerIndex)
			return;

        base.On_TouchEnd(touch);

        assignedFingerIndex = -1;

        levelEntity.OnFinishedMoving();
    }

    public override void SetColor(Color color)
    {
        sprite.color = color;
    }

    private void On_EntityMoved(LevelEntity movingEntity)
    {
        if (movingEntity != levelEntity)
            return;

        Vector2 angleNormalized = (transform.position - levelEntity.gizmo.transform.position).normalized;
        angleNormalized = angleNormalized.RoundVector2ToInt();
        Vector2 worldPos = ClampPositionToInsideRect(transform.position);
        if (worldPos == (Vector2) levelEntity.gizmo.transform.position)
        {
            worldPos -= angleNormalized;
        }

        Vector2 newOffset = worldPos - (Vector2)levelEntity.gizmo.transform.position;

        position = newOffset;
        transform.position = worldPos;
        if (On_PositionMoved != null)
            On_PositionMoved();
    }

    private void OnEnable()
    {
        TouchInput.On_TouchStart += On_TouchStart;
        TouchInput.On_TouchMaintained += On_TouchMaintained;
        TouchInput.On_TouchEnd += On_TouchEnd;

        LevelEntity.On_EntityMoved += On_EntityMoved;
    }

    private void Unsubscribe()
    {
        TouchInput.On_TouchStart -= On_TouchStart;
        TouchInput.On_TouchMaintained -= On_TouchMaintained;
        TouchInput.On_TouchEnd -= On_TouchEnd;

        LevelEntity.On_EntityMoved -= On_EntityMoved;
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
