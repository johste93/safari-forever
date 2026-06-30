using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SafariForever.Toolbar;
using DG.Tweening;

public class LogicCanvas : Singleton<LogicCanvas>
{
    public LinkDestroyer linkDestroyer;

    public GameObject overlay;

    public delegate void LogicCanvasUpdate(bool visible);
    public static LogicCanvasUpdate On_LogicCanvasUpdate;
    
    private static bool visible;
    public RectTransform backgroundRect;

    public Button openButton;
    public Button closeButton;

    public Canvas canvas;

    private void Start()
    {
        backgroundRect.anchoredPosition = new Vector2(backgroundRect.rect.width, backgroundRect.anchoredPosition.y);
    }

    public void ShowLogicCanvas(bool instant = false)
    {
        if(RailCanvas.RailsVisible())
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

        NodeManager.RevealNodes?.Invoke(NodeType.Output, null);

        overlay.SetActive(true);
        linkDestroyer.enabled = true;

        if(On_LogicCanvasUpdate != null)
            On_LogicCanvasUpdate(true);

        backgroundRect.DOKill();
        backgroundRect.DOAnchorPosX(0f, instant ? 0f : 0.3f).SetEase(Ease.OutQuad);
    }

    public void HideLogicCanvas(bool instant = false)
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

        NodeManager.HideNodes?.Invoke();

        linkDestroyer.enabled = false;

        if(On_LogicCanvasUpdate != null)
            On_LogicCanvasUpdate(false);

        backgroundRect.DOKill();
        backgroundRect.DOAnchorPosX(backgroundRect.rect.width, instant ? 0f : 0.3f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            overlay.SetActive(false);
            canvas.sortingOrder = -4;
        });
    }

    public static bool LogicVisible()
    {
        return visible;
    }

    private void On_EnterPlayMode()
    {
        HideLogicCanvas(true);
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
            ShowLogicCanvas();
        else
            HideLogicCanvas();
    }

    private void On_OrientationChanged(DeviceOrientation orientation)
    {
        backgroundRect.anchoredPosition = new Vector2(backgroundRect.rect.width, backgroundRect.anchoredPosition.y);
        HideLogicCanvas(true);
    }

    private void OnEnable()
    {
        ScreenOrientationManager.On_OrientationChanged += On_OrientationChanged;

        GameMaster.On_EnterPlayMode += On_EnterPlayMode;
		GameMaster.On_ExitPlayMode += On_ExitPlayMode;

        LevelBuilder.On_ChangedRoom += On_ChangedRoom;
    }

    private void Unsubscribe()
    {
        ScreenOrientationManager.On_OrientationChanged -= On_OrientationChanged;

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
