using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RestartButton : MonoBehaviour
{
	public CanvasGroup canvasGroup;
	public RectTransform anchorTransform;

	public void OnClick()
	{
		Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
		anchorTransform.DOComplete();
        anchorTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);

		GameMaster.instance.ResetLevel(true);
	}

	private void On_EnterPlayMode()
	{
		if(GameMaster.instance.GetCurrentMode() == GameMode.Create)
			return;

		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1;
	}

	private void On_ExitPlayMode()
	{
		if(GameMaster.instance.GetCurrentMode() == GameMode.Create)
			return;
			
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0;
	}

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
		GameMaster.On_ExitPlayMode += On_ExitPlayMode;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
		GameMaster.On_ExitPlayMode -= On_ExitPlayMode;
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
		anchorTransform.DOKill();
	}
}
