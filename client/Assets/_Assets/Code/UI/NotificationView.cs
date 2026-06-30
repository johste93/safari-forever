using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SafariForever.Notifications;

public class NotificationView : PopupView
{
    public NotificationList notificationList;

    public override void Show(System.Action onExit, bool instant = false, System.Action OnComplete = null)
    {
        SaveManager.currentSave.FetchOnlineProfile((profile)=>{
            if(profile == null)
                return;

            ColorUtility.TryParseHtmlString(profile.color, out Color c);
            PopupCanvas.instance.background.DOKill();
            PopupCanvas.instance.background.DOColor(c, 0.3f);
        });

        base.Show(onExit, instant, ()=>{
            OnComplete?.Invoke();
            notificationList.LoadNotifications();
        });
    }

    public void OnClickClose()
    {
        base.Exit();
    }
}
