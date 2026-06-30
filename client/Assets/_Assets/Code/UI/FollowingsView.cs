using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FollowingsView : PopupView
{
    public PlayerList playerList;

    private string userId;

    public void Initalize(string userId)
    {
        this.userId = userId;
        playerList.widgetHeader.text = "";
        playerList.ClearList();
    }

    public override void Show(System.Action onClose, bool instant = false, System.Action OnComplete = null)
    {
        //PopupCanvas.instance.background.DOKill();
        //PopupCanvas.instance.background.DOColor(Camera.main.backgroundColor, 0.3f);

        base.Show(onClose, instant, ()=>{
            OnComplete?.Invoke();
            playerList.Initalize(userId);
            playerList.LoadFollows();
        });
    }

    public void OnClickClose()
    {
        base.Exit();
    }
}
