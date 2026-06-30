using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Delay : MonoBehaviour, ISuspendable
{
    private LevelEntity entity;

    public Transform child;
    public Transform topSand, bottomSand;
    public GameObject topStream, bottomStream;

    private bool upsideDown;

    private float timeLeft = -1;

    private bool isPowered;

	private bool isSuspended;
	private Tween tween;
	private Tween rotationTween;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
    }

    private void Update()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

		if(isSuspended)
			return;

        if(timeLeft < 0f)
        {
            entity.outputNode.EmitPower(false);
            return;
        }

        timeLeft -= Time.deltaTime * SaveManager.currentSave.gameSpeed;

        UpdateSand();

        if(timeLeft < 0f)
        {
            tween?.Complete();
            tween = transform.DOPunchScale(Vector2.one * 0.3f, 0.3f, 1);
            entity.outputNode.EmitPower(true);
        }
        else
        {
            entity.outputNode.EmitPower(false);
        }
    }

    private void LateUpdate()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

        if(entity.inputNode.IsPowered())
        {
            if(!isPowered)
            {
                isPowered = true;
                StartCountdown();
            }
        }
        else
        {
            if(isPowered)
            {
                isPowered = false;
            }
        }
    }

    private void StartCountdown()
    {
        rotationTween?.Complete();
        rotationTween = child.DORotate(Vector3.forward * 180f, 0.3f, RotateMode.FastBeyond360).SetRelative(true);

        upsideDown = !upsideDown;

        timeLeft = ((NumberGizmo) entity.gizmo).GetValue();
    }

    private void UpdateSand()
    {
        float t = Mathf.Clamp01( timeLeft / ((NumberGizmo) entity.gizmo).GetValue());

        topStream.SetActive(upsideDown);
        bottomStream.SetActive(!upsideDown);

        if(upsideDown)
        {
            topSand.localPosition = new Vector2(0, 0.5f);
            bottomSand.localPosition = new Vector2(0, 0f);

            topSand.localScale = new Vector2(topSand.localScale.x, -0.5f * (1f-t));       
            bottomSand.localScale = new Vector2(bottomSand.localScale.x, -0.5f * t);
        }
        else
        {
            topSand.localPosition = new Vector2(0, 0f);
            bottomSand.localPosition = new Vector2(0, -0.5f);

            topSand.localScale = new Vector2(topSand.localScale.x, 0.5f * t);       
            bottomSand.localScale = new Vector2(bottomSand.localScale.x, 0.5f * (1f-t));
        }
    }

    private void Reset()
    {
        isPowered = false;
        transform.localScale = Vector3.one;
        timeLeft = -1f;

        UpdateSand();
    }

    private void On_LevelReset(bool manual) => Reset();

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += Reset;
        GameMaster.On_ExitPlayMode += Reset;
        GameMaster.On_LevelReset += On_LevelReset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= Reset;
        GameMaster.On_ExitPlayMode -= Reset;
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
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		rotationTween?.Kill();
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
			rotationTween?.Pause();
		}
		else
		{
			tween?.Play();
			rotationTween?.Play();
		}
	}
}
