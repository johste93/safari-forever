using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WebSummary : PopupView
{
    public LevelPreview levelPreview;

    public void Initalize()
    {
        levelPreview.SetName("Safari Forever");
    }

    public override void Show(System.Action onExit, bool instant = false, System.Action onComplete = null)
    {
        PopupCanvas.instance.background.DOKill();
        PopupCanvas.instance.background.DOColor(LevelBuilder.instance.GetCurrentColors().main, 0.3f);
        base.Show(onExit, instant, onComplete);
    }

    public void Replay()
    {
        base.Exit(false, ()=>
        {
            GameMaster.instance.ReplayLevel();
        });   
    }

    public void Continue()
    {
        Application.OpenURL("https://safariforever.com");
    }
}
