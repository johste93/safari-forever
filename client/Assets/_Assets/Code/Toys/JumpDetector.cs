using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;

public class JumpDetector : MonoBehaviour, ISuspendable
{
    private FSM_CharacterController controller;
    private LevelEntity entity;
    public Rotate rotator;

    private bool waitingLogicEvent;
    private float cooldown;

	private bool isSuspended;
	private Tween tween;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
    }

    private void On_PlayerStartedRunning(FSM_CharacterController controller)
    {
        if(this.controller != null)
            return;

        this.controller = controller;

		rotator.enabled = controller == null;
    }

	private void Update()
	{
        if(!GameMaster.instance.IsPlaying())
            return;

		if(isSuspended)
			return;

        if(cooldown > 0f)
            cooldown -= Time.deltaTime * SaveManager.currentSave.gameSpeed;

        DoLogicUpdate();

		if(controller == null)
			return;

		Vector2 dirToPlayer = controller.centerPivot.position - transform.position;
		rotator.transform.up = Vector2.MoveTowards(rotator.transform.up, dirToPlayer, 5f * Time.deltaTime);
	}

    private void DoLogicUpdate()
    {
        if(waitingLogicEvent)
        {
            entity.outputNode.EmitPower(true);
            waitingLogicEvent = false;
            cooldown = 0.1f;
        }
        else
        {
            entity.outputNode.EmitPower(false);
        } 
    }

    private void Trigger()
    {
        if(cooldown > 0f)
            return;

        tween?.Complete();
        tween = transform.DOPunchScale(Vector2.one * 0.3f, 0.3f, 1);
        
        waitingLogicEvent = true;
    }


    private void OnPlayerJumped(FSM_CharacterController controller)
    {
        Trigger();
    }

    private void OnPlayerJumpedOnJumpPad(FSM_CharacterController controller)
    {
        Trigger();
    }


    private void Reset()
    {
        waitingLogicEvent = false;

        controller = null;

        rotator.enabled = false;
        rotator.transform.eulerAngles = new Vector3(0,0,45f);
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;

        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += Reset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;

        Jumping.OnPlayerJumped += OnPlayerJumped;
        JumpingOnJumpPad.OnPlayerJumpedOnJumpPad += OnPlayerJumpedOnJumpPad;
    }

    private void Unsubscribe()
    {
        GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;

        GameMaster.On_EnterPlayMode -= Reset;
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= Reset;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;

        Jumping.OnPlayerJumped -= OnPlayerJumped;
        JumpingOnJumpPad.OnPlayerJumpedOnJumpPad -= OnPlayerJumpedOnJumpPad;

        Reset();
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
		tween?.Kill();
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
			tween?.Pause();
		}
		else
		{
			tween?.Play();
		}
	}
}
