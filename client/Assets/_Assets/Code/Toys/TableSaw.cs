using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;
using SF.LogicSystem.v2;

public class TableSaw : MonoBehaviour, ISuspendable
{
    public Transform saw;
    public Collider2D sawCollider;

    //public SpikeTrapSpike spike;
    public BoxCollider2D boxCollider2D;

    private Tween extendSpikes;
    private LevelEntity entity;
    private Tween spikeAnimation;
    private bool playerHasStartedToRun;

    private static int lastFramePlayedSound = 0;
    private bool isExtended = false;

	private bool isSuspended;
    private int framesOfPower;


    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
    }

    private void Start()
    {
		saw.localPosition = new Vector2(0, 0.15f);
		
        if(entity.GetSerializableData().gizmoDirection.HasValue)
        {
            transform.eulerAngles = new Vector3(0,0,((Direction4)entity.GetSerializableData().gizmoDirection).ToDegree());
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
		if(!GameMaster.instance.IsPlaying())
			return;

        if(!Globals.gameConstants.whatIsPlayer.Contains(other.gameObject.layer))
            return;

        if(extendSpikes != null)
            return;

        if (!playerHasStartedToRun)
            return;

        if(entity.inputNode.HasConnections())
            return;

        float duration = 0.4f/SaveManager.currentSave.gameSpeed;

        Vector2 hitNormal = other.GetContact(0).normal;
        if((hitNormal * -1) == ((Direction4) entity.GetSerializableData().gizmoDirection).ToVector())
        {
            extendSpikes = DOVirtual.DelayedCall(duration, ()=>
            {
                ExtendSpikes();
				extendSpikes = DOVirtual.DelayedCall(1.2f/ (SaveManager.currentSave.gameSpeed + 0.1f), ()=>{
					WithdrawSpikes();
				});
            });
        }
    }

    private void ExtendSpikes()
    {
        if(isExtended)
            return;

        if(isSuspended)
            return;
            
        sawCollider.enabled = true;
        saw.localPosition = new Vector2(0, 0.6f);
        sawCollider.transform.localPosition = new Vector2(0, -0.05f);

        AnimateSpikes();
        isExtended = true;
    }


    private void AnimateSpikes()
    {
        float animationDistance = 0.3f;

        if(lastFramePlayedSound != Time.frameCount)
        {
            lastFramePlayedSound = Time.frameCount;
            Audio.Play( SFX.instance.level.spikeTrap.trigger.randomClip, Channel.Game ).SetPitch(Random.Range(0.8f, 1.2f));
        }

        spikeAnimation = sawCollider.transform.DOLocalMoveY(-animationDistance, (0.3f/SaveManager.currentSave.gameSpeed)).SetLoops(-1).SetEase(Ease.Linear).OnComplete(()=>
        {
            sawCollider.transform.localPosition = new Vector2(0, -0.05f);
        });
    }

    private void On_EnterPlayMode()
    {
        boxCollider2D.enabled = true;
        Reset();
    }

    private void On_ExitPlayMode()
    {
        boxCollider2D.enabled = false;
        Reset();
		sawCollider.enabled = false;
    }

    private void On_PlayerStartedRunning(FSM_CharacterController controller)
    {
        playerHasStartedToRun = true;
    }
    
    private void WithdrawSpikes()
    {
        if(!isExtended)
            return;

        extendSpikes?.Kill();

        spikeAnimation?.Kill();
        spikeAnimation = null;
            
        sawCollider.transform.localPosition = new Vector2(0, -0.05f);

        sawCollider.enabled = false;
        saw.localPosition = new Vector2(0, 0.15f);
        extendSpikes = null;
        isExtended = false;
    }

    private void Reset()
    {
        framesOfPower = 0;
        WithdrawSpikes();
        playerHasStartedToRun = false;
        KillAllTweens();
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

        if(!entity.inputNode.HasConnections())
            return;

        if(framesOfPower >= 2)
        {
            ExtendSpikes();
        }
        else
        {
            WithdrawSpikes();
        }
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;

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
        spikeAnimation?.Kill();
		spikeAnimation = null;

        extendSpikes?.Kill();
        extendSpikes = null;
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
			spikeAnimation?.Pause();
			extendSpikes?.Pause();
		}
		else
		{
			spikeAnimation?.Play();
			extendSpikes?.Play();
		}
	}
}
