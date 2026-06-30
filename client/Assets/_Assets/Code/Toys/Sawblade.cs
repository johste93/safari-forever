using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawblade : MonoBehaviour, ISuspendable
{
    public float speed = 5f;
    public CircleCollider2D circleCollider2D;
    public LineRenderer lineRenderer;
    public Transform child;

    private NumberGizmo numberGizmo;
    private Direction4Gizmo directionGizmo;

    private LevelEntity entity;
    private PositionGizmo positionGizmo;

    private Vector2 offset = new Vector2(0.5f, 0.5f);

    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2? targetPoint = null;

    private int framesOfPower;
	private bool isSuspended;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        positionGizmo = entity.GetComponentInChildren<PositionGizmo>();

        numberGizmo = entity.GetComponentInChildren<NumberGizmo>(true);
        directionGizmo = entity.GetComponentInChildren<Direction4Gizmo>(true);

        numberGizmo.Initalize(entity);
        directionGizmo.Initalize(entity);

        entity.Unsubscribe();
        entity.Subscribe();

        UpdateDirectionGizmo();

        pointA = entity.GetSerializableData().topRight - offset;
        pointB = entity.gizmo.transform.position + (Vector3) positionGizmo.position;
        lineRenderer.SetPositions(new Vector3[]{pointA, pointB});

        entity.gizmo = entity.GetWidth() == entity.GetHeight() ? (Gizmo)numberGizmo : (Gizmo)directionGizmo;
        
        numberGizmo.gameObject.SetActive(entity.GetWidth() == entity.GetHeight());
        directionGizmo.gameObject.SetActive(entity.GetWidth() != entity.GetHeight());

        RepositionSaw();
        UpdateSize();
    }

    private void UpdateSize()
    {
        int size = Mathf.RoundToInt(numberGizmo.GetValue())-2;
        transform.localScale = Vector3.one + (Vector3.one * 0.5f * size);

        switch(size)
        {
            case -1:
                circleCollider2D.radius = 0.4f;
                break;
            case 0:
                circleCollider2D.radius = 0.5f;
                break;
            case 1:
                circleCollider2D.radius = 0.667f;
                break;
            case 2:
                circleCollider2D.radius = 0.75f;
                break;
        }
    }

    private void UpdateLineRenderer()
    {
        pointB = positionGizmo.transform.position;
        lineRenderer.SetPositions(new Vector3[]{pointA, pointB});
    }

    private void On_EntityMoved(LevelEntity movedEntity)
    {
        if(movedEntity != entity)
            return;

        positionGizmo.position = positionGizmo.transform.localPosition;
        
        UpdateLineRenderer();
    }

    private void Update()
    {
        if(!targetPoint.HasValue)
            return;

		if(isSuspended)
			return;

        if(framesOfPower >= 2 || !entity.inputNode.HasConnections())
        {
            child.position = Vector2.MoveTowards(child.position, (Vector2)targetPoint, SaveManager.currentSave.gameSpeed * Time.deltaTime * speed);
            if(Vector2.Distance(child.position, (Vector2)targetPoint) < 0.01f)
            {
                if(targetPoint == pointA)
                    targetPoint = pointB;
                else
                    targetPoint = pointA;
            }
        }
    }

    private void On_EnterPlayMode()
    {
        circleCollider2D.enabled = true;
        targetPoint = pointB;
        Reset();
    }

    private void On_ExitPlayMode()
    {
        circleCollider2D.enabled = false;
        Reset();
        targetPoint = null;
    }

    private void UpdateDirectionGizmo()
    {
        Gizmo gizmo = (Gizmo)entity.GetDragHandle();

        if(entity.GetWidth() != entity.GetHeight())
        {
            directionGizmo.left = directionGizmo.right = entity.GetWidth() > entity.GetHeight();
            directionGizmo.up = directionGizmo.down = entity.GetWidth() < entity.GetHeight();

            if(
                (directionGizmo.direction == Direction4.Up && !directionGizmo.up) ||
                (directionGizmo.direction == Direction4.Right && !directionGizmo.right) ||
                (directionGizmo.direction == Direction4.Down && !directionGizmo.down) ||
                (directionGizmo.direction == Direction4.Left && !directionGizmo.left))
            {   
                directionGizmo.direction = directionGizmo.GetFirstAvailableDirection();
                directionGizmo.face.eulerAngles = new Vector3(0,0, directionGizmo.direction.ToDegree());
                directionGizmo.shadow.eulerAngles = new Vector3(0,0, directionGizmo.direction.ToDegree());
            }
        }
        
        Vector3 centerPos = entity.GetSerializableData().centerPosition;
        centerPos.z = gizmo.transform.position.z;
        gizmo.transform.position = centerPos;
    }

    private void On_EntityChangedSize(LevelEntity entity)
    {
        if(entity != this.entity)
            return; 

        UpdateDirectionGizmo();
    }    

    private void Reset()
    {
        RepositionSaw();
        framesOfPower = 0;
    }

    private void RepositionSaw()
    {
        child.position = pointA;
    }

    private void LateUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

        if(isSuspended)
			return;

        if(entity.inputNode.HasConnections() && entity.inputNode.IsPowered())
            framesOfPower++;
        else
            framesOfPower = 0;
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;

        LevelEntity.On_EntityChangedSize += On_EntityChangedSize;

        positionGizmo.On_PositionMoved += UpdateLineRenderer;
        LevelEntity.On_EntityMoved += On_EntityMoved;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;

        LevelEntity.On_EntityChangedSize -= On_EntityChangedSize;

        positionGizmo.On_PositionMoved -= UpdateLineRenderer;
        LevelEntity.On_EntityMoved -= On_EntityMoved;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;
	}
}
