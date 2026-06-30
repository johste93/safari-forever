using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TestButton : MonoBehaviour
{
    public RectTransform anchorTransform;

    public Sprite playSprite;
    public Sprite stopSprite;

    public Image icon;

    public void OnClick()
    {
        if(TransitionHole.instance.IsClosed())
            return;

        TouchInput.CancelTouch();

        Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
        anchorTransform.DOComplete();
        anchorTransform.DOPunchScale(Vector3.one * 0.1f, 0.3f);

        if(!GameMaster.instance.IsPlaying()) 
            GameMaster.instance.TestLevel();
        else
            GameMaster.instance.ExitPlayMode();
    }

    private void On_EnterPlayMode()
    {
        icon.sprite = stopSprite;
    }

    private void On_ExitPlayMode()
    {
        icon.sprite = playSprite;
    }

    public static void CLICK()
    {
        FindObjectOfType<TestButton>().OnClick();
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
