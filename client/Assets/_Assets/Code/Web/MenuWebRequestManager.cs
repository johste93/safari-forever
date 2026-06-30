using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafariForever.Notifications;

public class MenuWebRequestManager
{
    public static void DoWebRequests()
	{
		 //Cloudsaving.
        UserAPI.DownloadBackup((downloadSuccess)=>
        {
            UserAPI.UploadBackup(null);
        }, true);

		//Check if we have a level cached.
		DeepLinkSingleton.instance?.LoadCachedShareCode(null);
		DeepLinkSingleton.instance?.LoadCachedUserId(null);

		//Peek notifications.
		NotificationAPI.GetUnreadNotificationCount((notificationSuccess, count, profile)=>
		{
			MenuManager.instance?.profileButton?.TryShow();
		});
	}

	public static void FetchUnrecivedRewards(System.Action onComplete = null)
	{
		/*
		((RewardsView)PopupCanvas.instance.rewardsView).Initalise(new List<UnrecivedReward>(){ 
			new UnrecivedReward() {
				ChangeInBalance = 0,
				BalanceBefore = 0,
				BalanceAfter = 0,
				TransactionType = TransactionType.WonHat,
				Hat = (Hat) Random.Range(1, 20),
				LevelName = "",
			}
		});
		PopupCanvas.instance.rewardsView.Show(()=>
		{
			if(!PopupCanvas.instance.HasActiveViews(PopupCanvas.instance.rewardsView))
			{
				PopupCanvas.instance.CloseCanvas();
				onComplete?.Invoke();
			}
		});
		return;
		*/

		TransactionAPI.GetUnrecivedReward((success, rewardsList)=>
		{
			if(!success)
			{
				onComplete?.Invoke();
				return;
			}

			if(rewardsList.Count > 0)
			{
				((RewardsView)PopupCanvas.instance.rewardsView).Initalise(rewardsList);
				PopupCanvas.instance.rewardsView.Show(()=>
				{
					if(!PopupCanvas.instance.HasActiveViews(PopupCanvas.instance.rewardsView))
					{
						PopupCanvas.instance.CloseCanvas();
						onComplete?.Invoke();
					}
				});
			}
			else
			{
				onComplete?.Invoke();
			}

		}, true);
	}
}
