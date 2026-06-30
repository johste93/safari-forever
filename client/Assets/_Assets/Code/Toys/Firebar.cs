using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafariForever.Toolbar;
using FSM_CharacterController2D;

public class Firebar : MonoBehaviour, ISuspendable
{
    public LineRenderer lineRenderer;
    public GameObject firePrefab;
    public Transform pivot;

    private LevelEntity entity;
    private PositionGizmo positionGizmo;

    private bool isSuspended;
    private int framesOfPower;

    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 offset = new Vector2(0.5f, 0.5f);

    private const float speed = 0.25f;

    private bool trackPlayer;
    private FSM_CharacterController characterController;

    private List<Transform> fires;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        positionGizmo = entity.GetComponentInChildren<PositionGizmo>();

        pointA = entity.GetSerializableData().topRight - offset;
        pointB = entity.gizmo.transform.position + (Vector3) positionGizmo.position;

        SpawnFire();

        SetLineRendererVisibility(Toolbar.instance != null && Toolbar.instance.GetCurrentButtonIndex() == (int) entity.requiredTab);
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

        if(framesOfPower >= 2 || !entity.inputNode.HasConnections())    
            RotateAroundCenter();
    }

    private void RotateAroundCenter()
    {
        float deltaMovement = Time.deltaTime * SaveManager.currentSave.gameSpeed * speed * -(int)entity.GetSerializableData().gizmoRotation * 360f;
        pivot.eulerAngles += new Vector3(0,0,deltaMovement);

        foreach(Transform fireTransform in fires)
        {
            fireTransform.up = transform.up;
        }
    }

    private void SpawnFire()
    {
        fires = new List<Transform>();
        float length = Vector2.Distance(pointA, pointB);

        for(int i = 1; i <= length; i++)
        {
            GameObject fire = Instantiate(firePrefab, pivot);
            fire.transform.localPosition = GetFacingDirection() * (1 * i);
            fires.Add(fire.transform);
        }
    }

    private Vector2 GetFacingDirection()
    {
        return (pointB - pointA).normalized;
    }

    private void Reset()
    {
        framesOfPower = 0;
        pivot.eulerAngles = Vector3.zero;
        for(int i = 0; i < fires.Count; i++)
        {
            fires[i].localPosition = GetFacingDirection() * (1 * (i+1));
            fires[i].up = transform.up;
        }
    }

    private void DestroyFires()
    {
        for(int i = fires.Count-1; i >= 0; i--)
        {
            Destroy(fires[i].gameObject);
        } 
        fires = new List<Transform>();
    }

    private void SetLineRendererVisibility(bool visible)
    {
        lineRenderer.enabled = visible;
    }

    private void UpdatePosition()
    {
        pointB = entity.gizmo.transform.position + (Vector3) positionGizmo.position;
        lineRenderer.SetPositions(new Vector3[]{pointA, pointB});

        DestroyFires();

        SpawnFire();
    }

    private bool IsAimingAtPlayer()
    {
        return Vector2.Distance(pointB, characterController.centerPivot.position) < 0.1f;
    }

    private void On_TabChange(int tabIndex)
    {
        SetLineRendererVisibility(tabIndex == (int)entity.requiredTab);
    }

    private void On_EnterPlayMode()
    {
        SetLineRendererVisibility(false);
        Reset();
    }

    private void On_ExitPlayMode()
    {
        Reset();
        SetLineRendererVisibility(Toolbar.instance.GetCurrentButtonIndex() == (int) entity.requiredTab);
    }

    private void On_LevelReset(bool manual) => Reset();

    private void On_LogicCanvasUpdate(bool visible)
    {
        if(visible || GameMaster.instance.IsPlaying())
        {
            SetLineRendererVisibility(false); 
        }
        else
        {  
            SetLineRendererVisibility(Toolbar.instance.GetCurrentButtonIndex() == (int) entity.requiredTab);
        }      
    }

    private void On_PlayerStartedRunning(FSM_CharacterController controller)
    {
        characterController = controller;
        trackPlayer = IsAimingAtPlayer();
    }

    private void On_PlayerDied(FSM_CharacterController controller)
    {
        trackPlayer = false;
    }

    public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;
	}

    private void OnEnable()
    {
        GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        GameMaster.On_PlayerDied += On_PlayerDied;

        positionGizmo.On_PositionMoved += UpdatePosition;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;

        LogicCanvas.On_LogicCanvasUpdate += On_LogicCanvasUpdate;
        Toolbar.On_TabChange += On_TabChange;
    }

    private void Unsubscribe()
    {
        GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        GameMaster.On_PlayerDied -= On_PlayerDied;

        positionGizmo.On_PositionMoved -= UpdatePosition;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;

        LogicCanvas.On_LogicCanvasUpdate -= On_LogicCanvasUpdate;
        Toolbar.On_TabChange -= On_TabChange;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }	
}
