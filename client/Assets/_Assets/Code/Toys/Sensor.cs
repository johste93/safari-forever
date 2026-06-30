using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;

public class Sensor : MonoBehaviour, ISuspendable
{
	private FSM_CharacterController controller;

	public Transform graphic;
	private LevelEntity entity;
    private LayerMask mask;

	public Transform eye1;
	public Transform eye2;

	public SpriteRenderer eyeRenderer1;
	public SpriteRenderer eyeRenderer2;

	public Color off;
	public Color on;

	private bool waitingLogicEvent;

	private bool isSuspended;

	private Tween tween;
	private Tween colorTween;

	private float size1;
	private float size2;

	private float maxSize = 1.5f;

	private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        mask = mask.CombineLayerMask(Globals.gameConstants.whatIsPlayer);
        mask = mask.CombineLayerMask(LayerMask.GetMask("Enemy"));

		size2 = maxSize/2f;
    }

	private void On_PlayerStartedRunning(FSM_CharacterController controller)
    {
        if(this.controller != null)
            return;

        this.controller = controller;
    }

	private void Update()
	{
		if(isSuspended)
			return;

		
		size1 += Time.deltaTime * 2f;

		eye1.transform.localScale = new Vector3(size1, size1, 1f);

		eyeRenderer1.color = eyeRenderer1.color.SetAlpha( EaseCurve.InExpo(1f, 0f, size1/maxSize) );

		if(size1 > maxSize)
			size1 = 0f;

		size2 += Time.deltaTime * 2f;

		eye2.transform.localScale = new Vector3(size2, size2, 1f);

		eyeRenderer2.color = eyeRenderer2.color.SetAlpha( EaseCurve.InExpo(1f, 0f, size2/maxSize) );

		if(size2 > maxSize)
			size2 = 0f;
		

		if(!GameMaster.instance.IsPlaying())
            return;

		DoLogicUpdate();
		
		/*
		if(controller == null)
			return;
		
		Vector2 dirToPlayer = controller.transform.position - transform.position;
		eye.transform.localPosition = Vector2.MoveTowards(eye.transform.localPosition, dirToPlayer.normalized * 0.125f, 5f * Time.deltaTime);
		//rotator.transform.up = Vector2.MoveTowards(rotator.transform.up, dirToPlayer, 5f * Time.deltaTime);
		*/
	}

	private void DoLogicUpdate()
    {
        if(waitingLogicEvent)
        {
            entity.outputNode.EmitPower(true);
            waitingLogicEvent = false;

			/*
			colorTween = DOVirtual.DelayedCall(0.4f, ()=>{
				eyeSprite.color = off;
			});
			*/
        }
        else
        {
            entity.outputNode.EmitPower(false);
        } 
    }

	public void OnTriggerEnter2D(Collider2D other)
	{
        if (!GameMaster.instance.IsPlaying())
            return;

		//Debug.Log(other.gameObject.name);
		if(!mask.Contains(other.gameObject.layer))
			return;

		SendSignal();
	}

	private void SendSignal()
	{
		waitingLogicEvent = true;

		//colorTween?.Kill();
		//eyeSprite.color = on;

		tween?.Complete();
        tween = graphic.DOPunchScale(Vector2.one * 0.2f, 0.3f, 1);
	}

	private void Reset()
    {
        waitingLogicEvent = false;
		//eyeSprite.color = off;
		//colorTween?.Kill();
    }

	private void On_LevelReset(bool manual) => Reset();


	private void OnEnable()
    {
		GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;

        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += Reset;
		
		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
		GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;

        GameMaster.On_EnterPlayMode -= Reset;
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= Reset;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
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
		colorTween?.Kill();
	}


	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		this.isSuspended = suspend;

		if(suspend)
			tween?.Pause();
		else
			tween?.Play();
	}
}
