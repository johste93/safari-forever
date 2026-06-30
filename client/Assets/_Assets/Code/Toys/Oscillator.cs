using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Diagnostics;

public class Oscillator : MonoBehaviour, ISuspendable
{
    public Transform pointer;

    private LevelEntity entity;
    private bool waitingLogicEvent;

	private bool isSuspended;

	private Tween tween;

    private int framesOfPower;

    private float time;

    private System.Diagnostics.Stopwatch stopwatch;

    private void Awake()
    {
        entity = GetComponentInParent<LevelEntity>();
        stopwatch = new System.Diagnostics.Stopwatch();
    }

    private void Update()
    {
        if(!GameMaster.instance.IsPlaying())
            return;

		if(isSuspended)
			return;

        DoLogicUpdate();

        if(framesOfPower < 2 && entity.inputNode.HasConnections())
            return;

        time += Time.deltaTime * SaveManager.currentSave.gameSpeed;
        
        float interval = ((NumberGizmo) entity.gizmo).GetValue();
        if(time > interval)
        {
            waitingLogicEvent = true;
            time = 0;
            tween?.Complete();
            tween = transform.DOPunchScale(Vector2.one * 0.3f, 0.3f, 1);
        }

        float position = Mathf.Clamp01(time / interval);
        pointer.eulerAngles = new Vector3(0,0, -position * 360f);
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
    }

    private void DoLogicUpdate()
    {
        if(waitingLogicEvent)
        {
            entity.outputNode.EmitPower(true);
            waitingLogicEvent = false;
        }
        else
        {
            entity.outputNode.EmitPower(false);
        } 
    }

    private void Reset()
    {
        framesOfPower = 0;
        waitingLogicEvent = false;
        time = 0;
        transform.localScale = Vector3.one;
        pointer.eulerAngles = Vector3.zero;
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
			tween?.Pause();
		else
			tween?.Play();
	}
}
