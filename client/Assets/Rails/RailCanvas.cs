using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SafariForever.Toolbar;
using DG.Tweening;

public class RailCanvas : Singleton<RailCanvas>
{
    public LinkDestroyer linkDestroyer;

    public GameObject overlay;

    public delegate void RailCanvasUpdate(bool visible);
    public static RailCanvasUpdate On_RailCanvasUpdate;
    
    private static bool visible;
    public RectTransform backgroundRect;

    public Button openButton;
    public Button closeButton;

    public Canvas canvas;

    private void Start()
    {
        backgroundRect.anchoredPosition = new Vector2(backgroundRect.rect.width, backgroundRect.anchoredPosition.y);
    }

    public void ShowRailCanvas(bool instant = false)
    {
        if(LogicCanvas.LogicVisible())
            return;

        if(visible)
            return;

        if(Toolbar.instance != null)
            Toolbar.instance.HideToolbar();

        if(!instant)
        {
            Audio.Play(SFX.instance.ui.tabChange.randomClip, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
            //Audio.Play(SFX.instance.level.logicSwitch.beep.randomClip, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
        }

        canvas.sortingOrder = -3;

        TouchInput.CancelTouch();

        openButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(true);

        visible = true;

        RailNodeManager.RevealNodes?.Invoke(NodeType.Output, null);

        overlay.SetActive(true);
        linkDestroyer.enabled = true;

        if(On_RailCanvasUpdate != null)
            On_RailCanvasUpdate(true);

        backgroundRect.DOKill();
        backgroundRect.DOAnchorPosX(0f, instant ? 0f : 0.3f).SetEase(Ease.OutQuad);
    }

    public void HideRailCanvas(bool instant = false)
    {
        if(!visible)
            return;

        if(Toolbar.instance != null)
            Toolbar.instance.ShowToolbar();

        if(!instant)
        {
            Audio.Play(SFX.instance.ui.tabChange.randomClip, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
            //Audio.Play(SFX.instance.level.logicSwitch.beep.randomClip, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
        }

        TouchInput.CancelTouch();

        openButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(false);

        visible = false;

        RailNodeManager.HideNodes?.Invoke();

        linkDestroyer.enabled = false;

        if(On_RailCanvasUpdate != null)
            On_RailCanvasUpdate(false);

        backgroundRect.DOKill();
        backgroundRect.DOAnchorPosX(backgroundRect.rect.width, instant ? 0f : 0.3f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            canvas.sortingOrder = -4;
            overlay.SetActive(false);
        });
    }

    public static bool RailsVisible()
    {
        return visible;
    }

    private void On_EnterPlayMode()
    {
        HideRailCanvas(true);
        backgroundRect.gameObject.SetActive(false);
    }

    private void On_ExitPlayMode()
    {
        backgroundRect.gameObject.SetActive(true);
    }

    private void On_ChangedRoom()
    {
        if(GameMaster.instance.IsPlaying())
            return;

        if(visible)
            ShowRailCanvas();
        else
            HideRailCanvas();
    }

    private void OnEnable()
    {
        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
		GameMaster.On_ExitPlayMode += On_ExitPlayMode;

        LevelBuilder.On_ChangedRoom += On_ChangedRoom;
    }

    private void Unsubscribe()
    {
        GameMaster.On_EnterPlayMode -= On_EnterPlayMode;
		GameMaster.On_ExitPlayMode -= On_ExitPlayMode;

        LevelBuilder.On_ChangedRoom -= On_ChangedRoom;
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
		backgroundRect.DOKill();
	}
}
