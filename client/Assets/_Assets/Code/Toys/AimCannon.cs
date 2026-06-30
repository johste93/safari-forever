using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SafariForever.Toolbar;
using FSM_CharacterController2D;

public class AimCannon : MonoBehaviour, ISuspendable
{
    public LineRenderer lineRenderer;
    public GameObject bulletPrefab;
    public Transform child;

    private float maxDelay = 2.5f;
    private float t;

    private LevelEntity entity;
    private bool isPlayMode;

    private PositionGizmo positionGizmo;

    private Tween positionTween;
    private Tween scaleTween;

    private float cooldown;
	private bool isSuspended;
    private bool isPowered;

    private static int lastFramePlayedSound = 0;

    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 offset = new Vector2(0.5f, 0.5f);

    private bool trackPlayer = false;
    private FSM_CharacterController characterController;

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

    private void LateUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
            return;
        
		if(isSuspended)
			return;

        if(trackPlayer)
        {
            pointB = (Vector2)characterController.centerPivot.position;
            Vector2 dirToTarget = pointB - pointA;
		    child.transform.up = Vector2.MoveTowards(child.transform.up, dirToTarget, 5f * Time.deltaTime);
        }

        if(entity.inputNode.HasConnections())
        {
            if(cooldown > 0)
            {
                cooldown -= Time.deltaTime * SaveManager.currentSave.gameSpeed;
                return;
            }

            if(entity.inputNode.IsPowered())
            {
                if(!isPowered)
                {
                    isPowered = true;
                    Fire();
                    t = 0f;
                    return;
                }
            }
            else
            {
                if(isPowered)
                {
                    isPowered = false;
                    t = 0;
                }
                return;
            }
        }

        t += Time.deltaTime * SaveManager.currentSave.gameSpeed;
        
        if(t > maxDelay)
        {
            Fire();
            t = 0;
        }
    }

    private void Fire()
    {
        if(lastFramePlayedSound != Time.frameCount)
        {
            lastFramePlayedSound = Time.frameCount;
            Audio.Play(SFX.instance.level.cannon.fire, Channel.Game);
        }

        GameObject lastSpawn = Instantiate(bulletPrefab, transform.position, Quaternion.identity, transform);
        Bullet lastBullet = lastSpawn.GetComponent<Bullet>();
        lastBullet.Initalize(GetFacingDirection());

        child.DOComplete();
        positionTween = child.DOLocalMove(-child.up * 0.33f, 0f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            positionTween = child.DOLocalMove(Vector2.zero, 0.5f).SetEase(Ease.OutQuad);
        });
        scaleTween = child.DOScale(new Vector2(1.3f, 0.6f), 0.1f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            scaleTween = child.DOScale(Vector2.one, 0.2f).SetEase(Ease.Linear);
        });

        cooldown = 0.25f * SaveManager.currentSave.gameSpeed;


        if(Bullet.GetBulletCount() > Globals.gameConstants.maxNumberOfBullets)
        {
            Bullet.KillFirst();
        }
    }

    private void Reset()
    {
        pointB = entity.gizmo.transform.position + (Vector3) positionGizmo.position;
        float angle = GetFacingDirection().ToAngle();
        child.eulerAngles = new Vector3(0,0, -angle);

        trackPlayer = false;
        isPowered = false;
        t = 0f;
        cooldown = 0f;

        if(positionTween != null)
        {
            positionTween.Kill();
            positionTween = null;
        }

        if(scaleTween != null)
        {
            scaleTween.Kill();
            scaleTween = null;
        }

        child.localScale = Vector3.one;
        child.localPosition = Vector3.zero;
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
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        GameMaster.On_LevelReset += On_LevelReset;
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
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        GameMaster.On_LevelReset -= On_LevelReset;
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
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		positionTween?.Kill();
		scaleTween?.Kill();
	}

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;
		if(suspend)
		{
			positionTween?.Pause();
			scaleTween?.Pause();
		}
		else
		{
			positionTween?.Play();
			scaleTween?.Play();
		}
	}
}
