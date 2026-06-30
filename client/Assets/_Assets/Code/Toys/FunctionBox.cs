using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;

public class FunctionBox : MonoBehaviour, IHeadSmashable, ISuspendable
{    
    public float resetDuration = 0.2f;

    protected SpriteRenderer[] spriteRenderers;
	protected Transform child;
    //private BoxCollider2D boxCollider2D;

	protected LevelEntity entity;
	
	private List<Tween> tweens = new List<Tween>();

    protected virtual void Awake()
	{
		entity = GetComponentInParent<LevelEntity>();
		child = transform.GetChild(0);
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        //boxCollider2D = GetComponent<BoxCollider2D>();
	}

	public void OnHeadSmashed(Character character)
	{
		if(character.controller.stateController.currentState.Equals(State.WallSliding))
			character.controller.motion.runningDirection *= -1;

		character.controller.stateController.OverideState(State.Falling);

        Jump();

		OnHit(character.controller);
	}

    protected virtual void Jump()
    {
		tweens = new List<Tween>();
		
        foreach (SpriteRenderer sR in spriteRenderers)
        {
            sR.color = sR.color.SetAlpha(0.25f);
        }

        float duration = 0.1f/(SaveManager.currentSave.gameSpeed + 0.1f);
        child.DOComplete();
        child.localScale = Vector3.one * 1.25f;
        tweens.Add(child.DOLocalMove(Vector3.up * 0.4f, 0).SetEase(Ease.Linear).OnComplete(() =>
        {
            tweens.Add(child.DOScale(Vector3.one, duration).SetEase(Ease.Linear));
            tweens.Add(child.DOLocalMove(Vector2.zero, duration).SetEase(Ease.Linear));
        }));

		tweens.Add(DOVirtual.DelayedCall(duration, ()=>{
			Revert();
		}));
    }

    protected virtual void OnHit(FSM_CharacterController characterController)
	{
		characterController.motion.rawVelocity.y = Mathf.Min(0f, characterController.motion.rawVelocity.y);
	}

	public virtual void Revert()
	{
		foreach(SpriteRenderer sR in spriteRenderers)
		{
			sR.color = sR.color.SetAlpha(1f);
		}

		child.localPosition = Vector3.zero;
	}

    public virtual void Reset()
	{
		Revert();
	}

	private void On_LevelReset(bool manual) => Reset();

	protected virtual void OnEnable()
	{
		GameMaster.On_ExitPlayMode += Reset;
		GameMaster.On_LevelReset += On_LevelReset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
	}

	protected virtual void Unsubscribe()
	{
		GameMaster.On_ExitPlayMode -= Reset;
		GameMaster.On_LevelReset -= On_LevelReset;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
	}

	protected virtual void OnDisable()
	{
		Unsubscribe();
	}

	protected virtual void KillAllTweens()
	{
		foreach(Tween t in tweens)
		{
			t?.Kill();
		}
	}

	protected virtual void OnDestroy()
	{
		Unsubscribe();

		KillAllTweens();
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
