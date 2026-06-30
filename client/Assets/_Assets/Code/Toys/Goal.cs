using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;
using DG.Tweening;

public class Goal : MonoBehaviour, ISuspendable
{
    public LayerMask whatIsPlayer;
    public BoxCollider2D boxCollider2D;
    public FlagPole flagPole;
    public Door door;
    public Transform pole;

    private List<Tween> tweens = new List<Tween>();
    private bool isDoor;
    private bool canceled;
    private bool goalActive;

    public delegate void GoalEvent();
    public static GoalEvent On_EnterGoal;
    public static GoalEvent On_ReachedGoalBase;

	private void OnTriggerEnter2D(Collider2D other)
	{
		OnTriggerStay2D(other);
	}

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!goalActive)
            return;

        if(!whatIsPlayer.Contains(other.gameObject.layer))
            return;

        FSM_CharacterController controller = other.GetComponent<FSM_CharacterController>();

        if(!controller.collisionInfo.below)
            return;

		if(controller.character.IsDead())
			return;

        if(controller.stateController.currentState.Equals(State.InsideBubble))
            return;

        goalActive = false;
        
        if(isDoor)
            EnterDoor(controller);
        else
            EnterGoal(controller);       
    }

    private void EnterGoal(FSM_CharacterController controller)
    {
		controller.boxCollider2D.enabled = false;
        controller.enabled = false;

        On_EnterGoal?.Invoke();

        MusicManager.DoFade(Globals.musicConstants.defaultVolume*0.1f, 0.3f);
            
        flagPole.Shake();
    
        Vector3 targetPostion = pole.position + new Vector3(0f,0.05f,0f);

        Tween t = controller.transform.DOMove(targetPostion, controller.properties.movementSpeed / 2f).SetSpeedBased(true).SetEase(Ease.OutQuad);
		t.OnComplete(()=>
        {
            if (canceled)
                return;

            On_ReachedGoalBase?.Invoke();

            controller.stateController.OverideState(State.Idle);
            
            tweens.Add(DOVirtual.DelayedCall(0.35f, ()=>
            {
                if (canceled)
                    return;

                flagPole.PlayAnimation();
                Audio.Play(SFX.instance.level.flagPole.backflip, Channel.Game);

                tweens.Add(DOVirtual.DelayedCall(0.9f/(SaveManager.currentSave.gameSpeed + 0.1f), ()=>
                {
                    if (canceled)
                        return;

                    DoBackflip(targetPostion, controller);

                    tweens.Add(DOVirtual.DelayedCall(1.5f, ()=>{
                        Audio.Play(SFX.instance.level.flagPole.fanfare, Channel.Game);
                    }));

                    tweens.Add(DOVirtual.DelayedCall(2f/(SaveManager.currentSave.gameSpeed + 0.1f), ()=>
                    {
                        System.Action callback = ()=> 
                        {    
                            if (canceled)
                                return;

                            //Point of no return?
                            GameMaster.instance.SetTransitionPointOfNoReturnReached();

                            MusicManager.DoFade(Globals.musicConstants.defaultVolume, 0.3f);

                            if (GameMaster.On_PlayerWon != null)
                                GameMaster.On_PlayerWon(controller);
                                
                            GameMaster.instance.CompleteLevel();  
                        };
                        
                        if(GifRecorder.instance.GetState() != RecordingState.ReadyToProcess)
                            callback();
                        else
                        {
                            DialogCanvas.instance.ShowRecordMenuWindow(callback);
                        }              
                    }));
                }));
            }));
        }).timeScale = SaveManager.currentSave.gameSpeed + 0.1f;
		
		tweens.Add(t);
    }

    private void EnterDoor(FSM_CharacterController controller)
    {
		controller.boxCollider2D.enabled = false;
        controller.enabled = false;

        On_EnterGoal?.Invoke();

        door.Open();

        Vector3 targetPostion = pole.position + new Vector3(0f,0.05f,0f);

        Tween t =
        controller.transform.DOMove(targetPostion, controller.properties.movementSpeed / 2f).SetSpeedBased(true).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            if (canceled)
                return;

            On_ReachedGoalBase?.Invoke();

            controller.stateController.OverideState(State.Idle);

            tweens.Add(DOVirtual.DelayedCall(0.35f, ()=>
            {
                if (canceled)
                    return;

                tweens.Add(DOVirtual.DelayedCall(0.9f/(SaveManager.currentSave.gameSpeed + 0.1f), ()=>
                {
                    System.Action callback = ()=> 
                    {    
                        if (canceled)
                            return;

                        //Point of no return?
                        GameMaster.instance.SetTransitionPointOfNoReturnReached();

                        if (GameMaster.On_PlayerWon != null)
                            GameMaster.On_PlayerWon(controller);

                        TransitionHole.instance.Close(transform);
                        tweens.Add(DOVirtual.DelayedCall(1f/(SaveManager.currentSave.gameSpeed + 0.1f), ()=>
                        {
                            GameMaster.instance.GoToNextRoom();
                        }));
                    };
                    
                    if(GifRecorder.instance.GetState() != RecordingState.ReadyToProcess)
                        callback();
                    else
                    {
                        DialogCanvas.instance.ShowRecordMenuWindow(callback);
                    }
                }));
            }));
        });
        t.timeScale = SaveManager.currentSave.gameSpeed;
        tweens.Add(t);
    }

    public void CancelGoalTransition()
    {
        if (GameMaster.instance.TransitionPointOfNoReturnReached())
            return;

        canceled = true;
        
        MusicManager.DoFade(Globals.musicConstants.defaultVolume, 0.3f);
        KillAllTweens();
    }

    private void DoBackflip(Vector3 targetPostion, FSM_CharacterController controller)
    {
        if (canceled)
            return;

        Transform container = new GameObject().transform;
        container.SetParent(controller.transform.parent);
        container.position = targetPostion;
        controller.transform.SetParent(container);

        float jumpDuration = 0.8f;
        {   //Jump
            if (canceled)
                return;

            tweens.Add(container.DOMove(targetPostion + Vector3.up * 4f, jumpDuration/2f).SetEase(Ease.Linear).OnComplete(()=>
            {
                if (canceled)
                    return;

                tweens.Add(container.DOMove(targetPostion, jumpDuration/2f).SetEase(Ease.Linear).OnComplete(()=>
                {
                    if (canceled)
                        return;

                    Audio.Play(SFX.instance.character.land, Channel.Player);
                }));
            }));

            tweens.Add(container.DOLocalRotate(new Vector3(0f, 0f, 360f * controller.motion.runningDirection), jumpDuration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine).SetRelative(true));
        }
    }

    private void UpdateGoalObject()
    {
        bool isLastFilledRoom = LevelBuilder.instance.GetCurrentRoomIndex() == LevelBuilder.instance.GetLastRoomIndex();
        pole.gameObject.SetActive(isLastFilledRoom);
        door.gameObject.SetActive(!isLastFilledRoom);
        isDoor = !isLastFilledRoom;
    }

    private void On_EnterPlayMode()
    {
        boxCollider2D.enabled = true;
    }

    private void On_ExitPlayMode()
    {
        Reset();
        boxCollider2D.enabled = false;
    }

    private void Reset()
    {
        goalActive = false;
        boxCollider2D.enabled = true;

        for(int i = 0; i < tweens.Count; i++)
            if(tweens[i] != null)
                tweens[i].Complete();

        tweens = new List<Tween>();
    }

    private void On_PlayerStartedRunning(FSM_CharacterController controller)
    {
        goalActive = true;
        canceled = false;
    }

    private void On_LevelReset(bool manual)
    {
        CancelGoalTransition();
    }

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;

        LevelBuilder.On_AddedRoom += UpdateGoalObject;
        LevelBuilder.On_DeletedRoom += UpdateGoalObject;

        GameMaster.On_PlayerStartedRunning += On_PlayerStartedRunning;
        GameMaster.On_LevelReset += On_LevelReset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;

        UpdateGoalObject();
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;

        LevelBuilder.On_AddedRoom -= UpdateGoalObject;
        LevelBuilder.On_DeletedRoom -= UpdateGoalObject;

        GameMaster.On_PlayerStartedRunning -= On_PlayerStartedRunning;
        GameMaster.On_LevelReset -= On_LevelReset;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();

		//Kill all Tweens.
		KillAllTweens();
    }

	private void KillAllTweens()
	{
        if (tweens == null)
            return;

		for(int i = 0; i < tweens.Count; i++)
            if(tweens[i] != null)
                tweens[i].Kill();

        tweens = new List<Tween>();
    }

	public void On_SuspensionEvent(bool isSuspended)
	{
		Suspend(isSuspended);
	}

	public void Suspend(bool suspend)
	{
		if (tweens == null)
            return;

		foreach(Tween t in tweens)
		{
			if(suspend)
				t?.Pause();
			else
				t?.Play();
		}
	}
}
