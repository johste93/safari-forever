using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using SafariForever.Notifications;

public class NotificationButton : MonoBehaviour
{
    public Image Icon;
    public TextMeshProUGUI TextMesh;

    private float cooldown;
    private Tween thisTween;

    public void OnClick()
    {
        PopupCanvas.instance.GetActivePopup().Close(false, ()=>{
            PopupCanvas.instance.notificationView.Show(()=>{
                PopupCanvas.instance.profileView.Show(null);
            });
        });
    }

    private void Update()
    {
        if(!NotificationAPI.cachedNotificationCount.HasValue)
            return;

        int count = NotificationAPI.cachedNotificationCount.Value;
        
        TextMesh.text = $"({ (count < 0 ? "?" : count.ToString()) })";

        if( NotificationAPI.cachedNotificationCount.Value > 0 )
        {
            if(cooldown > 0f)
            {
                cooldown -= Time.deltaTime;
            }
            else
            {
                Shake();
                cooldown = 0.8f;
            }
        }
    }

    private void Shake()
    {
        Icon.transform.DOKill();
        Icon.transform.eulerAngles = new Vector3(0,0,0);
        thisTween = Icon.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.3f).SetEase(Ease.InOutElastic);
    }

    private void KillAllTweens()
	{
		thisTween?.Kill();
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}	
}
