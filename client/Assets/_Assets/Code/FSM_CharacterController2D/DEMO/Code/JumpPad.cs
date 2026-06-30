using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;
using DG.Tweening;
using StandardCharacterController2D;
using StandardCharacterController2D.Spikeball.V1;

public class JumpPad : MonoBehaviour, ISuspendable
{
    public float baseForce = 7.2f;

    public Transform spring;
    public Transform pad;

    private LevelEntity entity;

    private bool waitingLogicEvent;

	private List<Tween> tweens = new List<Tween>();

    private int framOfLastSignalSent;

    private Direction8 direction
    {
        get
        {
            if (!entity.GetSerializableData().gizmoIndex.HasValue)
                entity.GetSerializableData().gizmoIndex = (int) Direction8.Up;

            return (Direction8) entity.GetSerializableData().gizmoIndex.Value;
        }
    }
        
    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        transform.eulerAngles = new Vector3(0,0, direction.ToDegree());
    }

    public void OnTriggerEnter2D(Collider2D other)
    {    
        if(other.enabled == false)
            return;

        if(!GameMaster.instance.IsPlaying())
            return;

        if( BouncePlayerOnEnter(other.GetComponent<FSM_CharacterController>()) ||
            BounceEnemyOnEnter(other.GetComponent<CharacterController2D>()))
        {
            Animate();
            waitingLogicEvent = true;
        }
    }

    private void Animate()
    {
        //float direction = Mathf.Sign(transform.position.x - other.transform.position.x);
		tweens = new List<Tween>();

        pad.DOComplete();
        tweens.Add(pad.DOPunchScale(Vector3.one * 0.4f, 0.3f, 1));
        pad.localPosition = new Vector2(0,0.125f);
        //pad.localEulerAngles = new Vector3(0,0, -20 * direction);
        tweens.Add(pad.DOLocalMoveY(-0.385f, 0.3f).SetEase(Ease.InCirc));
        //pad.DOLocalRotate(Vector3.zero, 0.3f).SetEase(Ease.InCirc);
        spring.DOComplete();
        spring.localScale = Vector2.one;
        tweens.Add(spring.DOScaleY(0.35f, 0.3f).SetEase(Ease.InCirc));

        Audio.Play( SFX.instance.level.jumpPad.jump.randomClip, Channel.Game );
    }

    private bool BounceEnemyOnEnter(CharacterController2D controller)
    {
        if(controller == null)
            return false;

        MotionController motionController = (MotionController) controller.MotionController;

        if (direction == Direction8.Left || direction == Direction8.Left_Down || direction == Direction8.Left_Up)
            motionController.MovingDirection = -1;
        else if (direction == Direction8.Right || direction == Direction8.Right_Down || direction == Direction8.Right_Up)
            motionController.MovingDirection = 1;

        //controller.StateController.SetState(new StandardCharacterController2D.Spikeball.V1.Falling());
        Vector2 bounceVelocity = direction.ToVector() * (CalculateForce() * 0.5f);
        bounceVelocity.x += ((Spikeball_CharacterController2D)controller).MovementSpeed * motionController.MovingDirection;
        motionController.SetInternalVelocity(bounceVelocity);

        return true;
    }

    private bool BouncePlayerOnEnter(FSM_CharacterController controller)
    {
        if(controller == null)
            return false;

        if(controller.character.IsDead())
            return false;

        if(controller.stateController.currentState.Equals(State.StuckInSlime) || controller.stateController.currentState.Equals(State.StuckInSlimeOnWall) || controller.stateController.currentState.Equals(State.StuckInSlimeOnRoof))
            return false;

        /*
        State previousState = controller.stateController.currentState.stateEnum;
	    if(previousState != State.Idle && previousState != State.Falling && previousState != State.FallingWithoutControl && previousState != State.FallingOfWall && previousState != State.Bouncing)
            return false;
        */
        if (!controller.stateController.currentState.Equals(State.WallSliding))
            if (direction.IsHorizontal() || direction.IsDiagonal())
            {
                if (direction == Direction8.Left || direction == Direction8.Left_Down || direction == Direction8.Left_Up)
                    controller.motion.runningDirection = -1;
                else if (direction == Direction8.Right || direction == Direction8.Right_Down || direction == Direction8.Right_Up)
                    controller.motion.runningDirection = 1;
            }

        controller.motion.rawVelocity = direction.ToVector() * CalculateForce();
        controller.stateController.OverideState(State.JumpingOnJumpPad);
        
        return true;
    }

    private void Update()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

        DoLogicUpdate();
    }

    private void DoLogicUpdate()
    {
        if(waitingLogicEvent)
        {
            if((Time.frameCount - framOfLastSignalSent) > 3)
            {
                framOfLastSignalSent = Time.frameCount;
                entity.outputNode.EmitPower(true);
            }
            waitingLogicEvent = false;
        }
        else
        {
            entity.outputNode.EmitPower(false);
        } 
    }

    private float CalculateForce()
    {
        switch(direction)
        {
            case Direction8.Up:
                return baseForce * 5;
            case Direction8.Left_Up:
            case Direction8.Right_Up:
                return baseForce * 4;
            case Direction8.Left:
            case Direction8.Right:
                return baseForce * 3;
            case Direction8.Right_Down:
            case Direction8.Left_Down:
                return baseForce * 2;
            default:
            case Direction8.Down:
                return baseForce;
        }
    }

    private void Reset()
    {
        waitingLogicEvent = false;
        framOfLastSignalSent = -2;
    }

    private void On_LevelReset(bool manual) => Reset();


    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += Reset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
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
		foreach(Tween t in tweens)
		{
			t?.Kill();
		}
	}

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		foreach(Tween t in tweens)
		{
			if(suspend)
				t?.Pause();
			else
				t?.Play();
		}
	}
}
