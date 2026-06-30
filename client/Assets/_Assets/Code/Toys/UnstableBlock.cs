using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSM_CharacterController2D;

public class UnstableBlock : MonoBehaviour, ISuspendable
{   

    public EdgeCollider2D edgeCollider;
    public Transform container;
    public float speed = 10f;
    public float resetDuration = 5f;
	private bool isTriggered;
	private bool isFalling;

    public SpriteRenderer[] spriteRenderers;

    public Vector2 defaultPosition;
    private Sequence blinkSequence;
    private Tween scaleTween;
	private Tween fallTween;
	private Tween shakeTween;
	private Tween delayTween;
	private List<Tween> tweens = new List<Tween>();

	private bool isSuspended;

    private void Start()
    {
        On_ColorPaletteChanged(LevelBuilder.instance.GetCurrentColors());
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if(!GameMaster.instance.IsPlaying())
            return;
            
        if(!Globals.gameConstants.whatIsPlayer.Contains(other.gameObject.layer))
            return;

        if(isTriggered)
            return;

        FSM_CharacterController controller = other.gameObject.GetComponent<FSM_CharacterController>();

        if(!controller.collisionInfo.below)
            return;

		Fall();
    }

    private void Blink(float duration)
    {
        blinkSequence = DOTween.Sequence();
        
        for(float f = 0; f < duration; f += 0.1f)
        {
            blinkSequence.Append(SetAlpha(0f, 0, 0.1f / SaveManager.currentSave.gameSpeed));
            blinkSequence.Append(SetAlpha(0.25f, 0, 0.1f / SaveManager.currentSave.gameSpeed));
        }
        
        blinkSequence.Append(SetAlpha(1, 0, 0.1f / SaveManager.currentSave.gameSpeed));

        blinkSequence.Play();
    }

    private Tween SetAlpha(float alpha, float duration, float delay)
    {
        return DOVirtual.DelayedCall(delay / SaveManager.currentSave.gameSpeed, ()=> {
            foreach(SpriteRenderer sr in spriteRenderers)
                sr.DOFade(alpha, duration);
        });
    }

	private void Fall()
    {
		isTriggered = true;
        shakeTween = container.DOShakePosition(1f / SaveManager.currentSave.gameSpeed, 0.2f, 10, 90, false, false).SetEase(Ease.Linear);
        delayTween = DOVirtual.DelayedCall(1f / SaveManager.currentSave.gameSpeed, ()=>
		{
			edgeCollider.enabled = false;
			isFalling = true;

			tweens = new List<Tween>();
			foreach(SpriteRenderer sR in spriteRenderers)
			{
				tweens.Add(sR.DOFade(0f, 0.3f));
			}
		});
    }

	private void Update()
    {
        if(isSuspended)
            return;

		if(!isFalling)
			return;
		
        Vector3 deltaMovement = Vector2.down * speed * SaveManager.currentSave.gameSpeed * Time.deltaTime;
        transform.position += deltaMovement;
    }


    private void Reset()
    {
		KillAllTweens();

		isTriggered = false;
		isFalling = false;
            
        container.localScale = Vector3.one;

        transform.localPosition = defaultPosition;
        edgeCollider.enabled = true;
        container.localPosition = Vector3.zero;  

		foreach(SpriteRenderer sR in spriteRenderers)
		{
			sR.color = sR.color.SetAlpha(1f);
		} 
    }

    private void On_EnterPlayMode()
    {
        Reset();
    }

    private void On_ExitPlayMode()
    {
        Reset();
        edgeCollider.enabled = false;
    }

    private void On_LevelReset(bool manual) => Reset();

    private void On_ColorPaletteChanged(ColorPalette colorPallete)
    {
        spriteRenderers[0].color = colorPallete.floor.SetVibrance(colorPallete.floor.GetVibrance()+0.10f); //Top
        spriteRenderers[1].color = colorPallete.floor.SetVibrance(colorPallete.floor.GetVibrance()-0.10f); //Wall
        spriteRenderers[2].color = colorPallete.floor; //Face
    }

    protected virtual void OnEnable()
    {
        LevelBuilder.On_ColorPaletteChanged += On_ColorPaletteChanged;

        GameMaster.On_LevelReset += On_LevelReset;
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    protected virtual void Unsubscribe()
    {
        LevelBuilder.On_ColorPaletteChanged -= On_ColorPaletteChanged;

        GameMaster.On_LevelReset -= On_LevelReset;
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;

		SuspensionManager.On_SuspensionEvent -= On_SuspensionEvent;
    }

    protected virtual void OnDisable()
    {
        Unsubscribe();
		KillAllTweens();
    }

    protected virtual void OnDestroy()
    {
        Unsubscribe();
    }

	private void KillAllTweens()
	{
		container.DOKill();
		blinkSequence?.Kill();
		scaleTween?.Kill(); 
		fallTween?.Kill();
		delayTween?.Kill();
		shakeTween?.Kill();
		
		foreach(Tween t in tweens)
		{
			t?.Kill();
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
		if(suspend)
		{
			scaleTween?.Pause();
			blinkSequence?.Pause();
			fallTween?.Pause();
			delayTween?.Pause();
			shakeTween?.Pause();
		}
		else
		{
			scaleTween?.Play();
			blinkSequence?.Play();
			fallTween?.Play();
			delayTween?.Play();
			shakeTween?.Play();
		}

		foreach(Tween t in tweens)
		{
			if(suspend)
				t?.Pause();
			else
				t?.Play();
		}
	}
}
