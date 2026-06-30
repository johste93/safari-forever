using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SafariForever.Notifications;

public class PushNotificationButton : MonoBehaviour
{
	public RectTransform child;

    public void OnClick()
	{
		child.DOComplete();
        child.DOPunchScale(Vector3.one * 0.1f, 0.3f).OnComplete(()=>
		{
			DialogCanvas.instance.ShowLoading();
			NotificationAPI.GetNotificationToggleStatus((success, status)=>
			{
				DialogCanvas.instance.HideLoading();

				if(!success)
					return;

				PopupView previousView = PopupCanvas.instance.GetActivePopup();
				previousView.Close(false, ()=>
				{							
					((NotificationOptionsView)PopupCanvas.instance.notificationOptionsView).Initalise(status);
					PopupCanvas.instance.notificationOptionsView.Show(()=>{
						previousView.Show(null);
					});
				});
			});	
		});	
	}

	private void OnDestroy()
    {
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		child.DOKill();
	}
}
