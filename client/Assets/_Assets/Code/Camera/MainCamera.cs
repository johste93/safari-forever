using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainCamera : Singleton<MainCamera>
{
    private Tween tween;

    public void PlayDeathEffect(System.Action callback)
    {
        Audio.Play( SFX.instance.ui.deathEffect.randomClip, Channel.UI);
        
        tween =
			DeathEffect.instance?.DoEffect(()=>
			{
				TransitionSingleton.instance.previousLevelColor = Camera.main.backgroundColor;
            	TransitionSingleton.instance.CloseBlinds(TransitionSingleton.instance.previousLevelColor, false, callback);
            }
		);
    }

    private void On_LevelReset(bool manual)
    {
       	tween?.Kill();
		tween = null;
        DeathEffect.instance?.UndoEffect();
    }

    private void On_ExitPlayMode()
    {
        tween?.Kill();
        tween = null;
        DeathEffect.instance?.UndoEffect();
    }

    private void OnEnable()
    {
        GameMaster.On_ExitPlayMode += On_ExitPlayMode;
        GameMaster.On_LevelReset += On_LevelReset;

		SuspensionManager.On_SuspensionEvent += On_SuspensionEvent;
    }

    private void Unsubscribe()
    {
        GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
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
        Time.timeScale = 1f;
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
		if(suspend)
			tween?.Pause();
		else
			tween?.Play();
	}
}
