using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using SafariForever.Notifications;

public class ProfileButton : MonoBehaviour
{
    public Sprite ProfileSprite;
    public Sprite NotificationSprite;
    public Image Icon;
    public TextMeshProUGUI NotificationCounterTextMesh;
    public CurrencyButton currencyButton;

    public CanvasGroup canvasGroup;

    private float cooldown;
    private Tween tween;
    private Tween notificationTween;
	private bool tryingToShow;

    private void Start()
    {
        NotificationAPI.On_NotificationCountUpdate += On_NotificationCountUpdate;
        NameChange.On_NameUpdate += On_NameUpdate;
    }
	
    private void On_NameUpdate()
    {
        if(NotificationAPI.cachedNotificationCount.HasValue)
            On_NotificationCountUpdate(NotificationAPI.cachedNotificationCount.Value);
    }

    private void On_NotificationCountUpdate(int count)
    {
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
                return;

            if(count > 0)
                ShowNotification(profile, count);
            else
                ShowDefault(profile);
        });
    }

    public void OnClick()
    {
        Audio.Play(SFX.instance.ui.flatButton, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));
        transform.DOComplete();
        transform.DOPunchScale(new Vector2(0.1f, 0.2f), 0.3f, 1);
        
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
                return;

            DialogCanvas.instance.ShowLoading();
            ((ProfileView)PopupCanvas.instance.profileView).Initalize(profile.userId, (success)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                    return;

                PopupCanvas.instance.profileView.Show(()=>{
                    PopupCanvas.instance.CloseCanvas();
                });
            });
        }); 
    }

    private void ShowNotification(Profile profile, int count)
    {
        NotificationCounterTextMesh.text = $"{profile.nickname} ({(count < 0 ? "?" : count.ToString())})";
        Icon.sprite = NotificationSprite;
        Icon.rectTransform.anchoredPosition = new Vector2(Icon.rectTransform.anchoredPosition.x, 0);
    }

    private void ShowDefault(Profile profile)
    {
        NotificationCounterTextMesh.text = profile.nickname;
        Icon.sprite = ProfileSprite;
        Icon.rectTransform.anchoredPosition = new Vector2(Icon.rectTransform.anchoredPosition.x, -4);
    }

    private void Update()
    {
        if(!NotificationAPI.cachedNotificationCount.HasValue)
            return;

        if( NotificationAPI.cachedNotificationCount.Value != 0 )
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
        notificationTween = Icon.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.3f).SetEase(Ease.InOutElastic);
    }

    public void TryShow()
    {
		if(tryingToShow)
			return;

		tryingToShow = true;
        bool failSilently = true;
        if(SaveManager.currentSave.HasOnlineProfile())
        {
			WebWorker worker = WebWorkerFactory.HireWorker(this);
            worker.CheckInternetConnection(failSilently, (isConnected, serverTime)=>
            {
                if(!isConnected)
				{
					tryingToShow = false;
					canvasGroup.interactable = false;
					return;
				}

				SaveManager.currentSave.FetchOnlineProfile((profile)=>
				{
					tryingToShow = false;

					if(profile == null)
					{
						canvasGroup.interactable = false;
						return;
					}

					tween = canvasGroup.DOFade(1f, 0.5f);
					canvasGroup.interactable = true;

					if(NotificationAPI.cachedNotificationCount.HasValue && NotificationAPI.cachedNotificationCount.Value > 0)
						ShowNotification(profile, NotificationAPI.cachedNotificationCount.Value);
					else
						ShowDefault(profile);

                    currencyButton.TryShow(profile.coins);
						
				}, true);   
			});
        }
		else
		{
			canvasGroup.interactable = false;
			tryingToShow = false;
		}
    }
    
    private void OnDestroy()
    {
		KillAllTweens();
        NotificationAPI.On_NotificationCountUpdate -= On_NotificationCountUpdate;
        NameChange.On_NameUpdate -= On_NameUpdate;
    }

	private void KillAllTweens()
	{	
		tween?.Kill();
        notificationTween?.Kill();
	}
}
