using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;

public class OneWayGate : MonoBehaviour, ISuspendable
{	
    public Transform door1;
	public Transform door2;
	public EdgeCollider2D edgeCollider2D;

	[HideInInspector]
	public LevelEntity entity;
	private bool isOpen;
	private Direction4 direction;

	private FSM_CharacterController player;

	private static int lastFramePlayedSound = 0;

	private bool powered = true;
	private int framesOfPower;
	private bool isPlayingOpeningAnim;
	private bool waitingCloseAnimation;
	private bool isSuspended;

	private Tween door1Tween;
	private Tween door2Tween;

	private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
    }

	private void Start()
    {
        if(entity.GetSerializableData().gizmoDirection.HasValue)
        {
            transform.eulerAngles = new Vector3(0,0,((Direction4)entity.GetSerializableData().gizmoDirection).ToDegree());

			switch((Direction4)entity.GetSerializableData().gizmoDirection)
			{
				case Direction4.Up:
					edgeCollider2D.tag = "OneWayGateNorth";
				break;
				case Direction4.Right:
					edgeCollider2D.tag = "OneWayGateEast";
				break;
				case Direction4.Down:
					edgeCollider2D.tag = "OneWayGateSouth";
				break;
				case Direction4.Left:
					edgeCollider2D.tag = "OneWayGateWest";
				break;
			}
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
    }

	private void On_PlayerStartedRunning(FSM_CharacterController controller)
	{
		player = controller;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if( !GameMaster.instance.IsPlaying() )
			return;

		if(other.isTrigger)
			return;

		Open();
	}

	private void UpdateAnimation(bool instant)
	{
		if(isPlayingOpeningAnim && !instant)
			return;
			
		if(!isOpen && waitingCloseAnimation)
		{
			CloseAnimation(instant);
		}
	}

	private void OpenAnimation(bool instant)
	{
		isPlayingOpeningAnim = true;
		door1Tween.Kill();
		door1Tween = door1.DOLocalRotate(new Vector3(0, 0, 90), instant ? 0f : 0.2f).SetEase(Ease.OutBack);
		door2Tween.Kill();
		door2Tween = door2.DOLocalRotate(new Vector3(0, 0, -90), instant ? 0f : 0.2f).SetEase(Ease.OutBack).OnComplete(()=>{
			isPlayingOpeningAnim = false;
		});
	}

	private void CloseAnimation(bool instant)
	{
		waitingCloseAnimation = false;
		door1Tween.Kill();
		door1Tween = door1.DOLocalRotate(new Vector3(0, 0, 0), instant ? 0f : 0.3f).SetEase(Ease.InBack);
		door2Tween.Kill();
		door2Tween = door2.DOLocalRotate(new Vector3(0, 0, 0), instant ? 0f : 0.3f).SetEase(Ease.InBack);
	}

	public void Open(bool instant = false)
	{
		if(isOpen && !instant)
			return;

		edgeCollider2D.enabled = false;
		isOpen = true;

		OpenAnimation(instant);

		if(!instant)
			if(lastFramePlayedSound != Time.frameCount)
			{
				lastFramePlayedSound = Time.frameCount;
				Audio.Play(SFX.instance.level.gate.open, Channel.Game);
			}
	}

	public void Close(bool instant = false)
	{
		if(!isOpen && !instant)	
			return;

		edgeCollider2D.enabled = true;
		isOpen = false;
		waitingCloseAnimation = true;

		if(!instant)
			if(lastFramePlayedSound != Time.frameCount)
			{
				lastFramePlayedSound = Time.frameCount;
				Audio.Play(SFX.instance.level.gate.close, Channel.Game);
			}
	}	

	private void Reset()
	{
		if(!GameMaster.instance.IsPlaying())
		{
			player = null;
			Close(true);
			UpdateAnimation(true);
			return;
		}
		
		this.DelayEndOfFrame(()=>
		{
			if(entity.inputNode.HasConnections())
			{
				if(entity.inputNode.IsPowered())
				{
					framesOfPower = 2;
					Close(true);
				}
				else
				{
					framesOfPower = 0;
					Open(true);
				}
			}
			else
			{
				Close(true);
			}

			player = null;
			UpdateAnimation(true);
		});
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

		if(entity.inputNode.HasConnections())
		{
			if(framesOfPower >= 2)
				Close();
			else
				Open();
		}

		UpdateAnimation(false);
    }

	private void On_LevelReset(bool manual) => Reset();

	private void OnEnable()
    {
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_ExitPlayMode += Reset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_EnterPlayMode -= Reset;
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
		door1Tween?.Kill();
		door2Tween?.Kill();
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
			door1Tween?.Pause();
			door2Tween?.Pause();
		}
		else
		{
			door1Tween?.Play();
			door2Tween?.Play();
		}
	}
}
