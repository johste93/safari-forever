using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SafariForever.Toolbar;
using FSM_CharacterController2D;

public class AimLaserCannon : MonoBehaviour, ISuspendable
{
    public LineRenderer lineRenderer;
    public Transform child;
    public Gradient gradient;

    public AimLaserGraphic laserGraphic;
    public AimLaserBeam beam;

    [HideInInspector]
    public bool isFiring;
    [HideInInspector]
    public bool isPreparing;
    [HideInInspector]
    public float startFireTime;

    private LevelEntity entity;

    public const float fireDuration = 1f;
    private const float maxDelay = 1f;
    public const float warmUpDuration = 1f;
    
    private float t;
    private float colorT;
    private AudioPlayer beamLoop;

    private int framesOfPower;

    private List<Tween> tweens = new List<Tween>();

    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 offset = new Vector2(0.5f, 0.5f);

    private bool trackPlayer = false;
    private FSM_CharacterController characterController;
    private PositionGizmo positionGizmo;
	
	private bool _isSuspended;
	public bool isSuspended {
		get{
			return _isSuspended;
		}
		private set{
			_isSuspended = value;
		}
	}

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>(); 
        positionGizmo = entity.GetComponentInChildren<PositionGizmo>();

        pointA = entity.GetSerializableData().topRight - offset;
        pointB = entity.gizmo.transform.position + (Vector3) positionGizmo.position;

        lineRenderer.SetPositions(new Vector3[]{pointA, pointB});
        
        float angle = GetFacingDirection().ToAngle();
        child.eulerAngles = new Vector3(0,0, -angle);

        SetLineRendererVisibility(Toolbar.instance != null && Toolbar.instance.GetCurrentButtonIndex() == (int) entity.requiredTab);

        colorT = Random.Range(0f, 1f);
        laserGraphic.SetColor(gradient.Evaluate(colorT), 0.5f);
    }

    private bool IsAimingAtPlayer()
    {
        return Vector2.Distance(pointB, characterController.centerPivot.position) < 0.1f;
    }

    private Vector2 GetFacingDirection()
    {
        return (pointB - pointA).normalized;
    }

    private void UpdatePosition()
    {
        pointB = entity.gizmo.transform.position + (Vector3) positionGizmo.position;
        lineRenderer.SetPositions(new Vector3[]{pointA, pointB});

        float angle = GetFacingDirection().ToAngle();
        child.eulerAngles = new Vector3(0,0, -angle);
    }

    private void Update()
    {		
		if(isSuspended)
			return;

        if(isFiring)
        {
            float time = Time.time * 3.75f;
            colorT = time - Mathf.FloorToInt(time);
        }

        laserGraphic.SetColor(gradient.Evaluate(colorT), isFiring ? 1f : 0.5f);    
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

        if(trackPlayer)
        {
            pointB = (Vector2)characterController.centerPivot.position;
            beam.AdjustLength(GetFacingDirection());
		    child.transform.up = Vector2.MoveTowards(child.transform.up, GetFacingDirection(), 0.4f * Time.deltaTime);
        }
        
        if(entity.inputNode.HasConnections())
        {
            if(framesOfPower >= 2)
            {
                if(framesOfPower == 2)
                    Fire();
            }
            else
            {
                Stop();
            }
        }
        else
        {
            if(!isFiring && !isPreparing)
                t += Time.deltaTime * SaveManager.currentSave.gameSpeed;

            if(t > maxDelay)
            {
                Prepare();
                t = 0;
            }
        }

        if(isFiring)
            beam.AdjustLength(GetFacingDirection());
    }

    private void Prepare()
    {
        KillAllTweens();
        Stop();

        isPreparing = true;
        beam.Prepare(GetFacingDirection());

        Audio.Play(SFX.instance.level.laser.warmup, Channel.Game);

        tweens.Add(DOVirtual.DelayedCall(warmUpDuration/(SaveManager.currentSave.gameSpeed+0.1f), ()=>
        {    
            Fire();
        }, false));
    }

    private void Fire()
    {
        isFiring = true;
        startFireTime = Time.time;
        isPreparing = false;

        beam.Prepare(GetFacingDirection());
        beam.Fire();

        beamLoop = Audio.Play(SFX.instance.level.laser.beam, Channel.Game)?.SetLoop(true).SetPitch(Random.Range(0.9f, 1.1f));

        if(!entity.inputNode.HasConnections())
        {
            tweens.Add(DOVirtual.DelayedCall(fireDuration/(SaveManager.currentSave.gameSpeed + 0.1f), ()=>
            {
                Stop();
            }, false)); 
        }
    }

    private void On_PlayerStartedRunning(FSM_CharacterController controller)
    {
        characterController = controller;
        trackPlayer = IsAimingAtPlayer();
    }
    
    private void SetLineRendererVisibility(bool visible)
    {
        lineRenderer.enabled = visible;
    }

    public void Stop()
    {
        isPreparing = false;
        isFiring = false;
        beamLoop?.Kill();
        beamLoop = null;
        beam.Stop();
    }

    private void Reset()
    {
        pointB = entity.gizmo.transform.position + (Vector3) positionGizmo.position;
        float angle = GetFacingDirection().ToAngle();
        child.eulerAngles = new Vector3(0,0, -angle);

        trackPlayer = false;
        t = 0f;

        framesOfPower = 0;

        Stop();

        KillAllTweens();

        if(GameMaster.instance.IsPlaying() && entity.inputNode.HasConnections())
		{
            this.DelayEndOfFrame(()=>
            {
                if(entity.inputNode.IsPowered())
                {
                    framesOfPower = 2;
                    Fire();
                }
            });
        }
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

    private void On_PlayerDied(FSM_CharacterController controller)
    {
        trackPlayer = false;
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_LevelReset += On_LevelReset;
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
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        GameMaster.On_PlayerDied -= On_PlayerDied;

        positionGizmo.On_PositionMoved -= UpdatePosition;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;

        LogicCanvas.On_LogicCanvasUpdate -= On_LogicCanvasUpdate;
        Toolbar.On_TabChange -= On_TabChange;
    }

    private void OnDestroy()
    {
        Unsubscribe();
        KillAllTweens();

        beamLoop?.Kill();
    }

    private void OnDisable()
    {
        Unsubscribe();
        KillAllTweens();
    }

    private void KillAllTweens()
    {
        if(tweens != null)
            foreach(Tween t in tweens)
            {
                if(t != null)
                    t.Kill();
            }

        tweens = new List<Tween>();
    }

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;

		if(tweens != null)
            foreach(Tween t in tweens)
            {
				if(suspend)
					t?.Pause();
				else
					t?.Play();
            }
	}
}
