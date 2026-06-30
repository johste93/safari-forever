using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafariForever.Notifications;

public class NotificationOptionsView : PopupView
{
    public PushToggle FollowedPlayedLevel;
    public PushToggle FollowedLikedLevel;
    public PushToggle FollowedPublishedLevel;
    public PushToggle MyLevelIsLevelOfTheWeek;
    public PushToggle NewFollower;
    public PushToggle WorldRecordBeaten;
    public PushToggle NewDailyChallenge;
    public PushToggle DailyChallengeResults; 

    private PushNotificationStatusResponse cachedStatus;

    private bool anyEnabled{
        get{
            return  FollowedPlayedLevel.GetValue() ||
                    FollowedLikedLevel.GetValue() ||
                    FollowedPublishedLevel.GetValue() ||
                    MyLevelIsLevelOfTheWeek.GetValue() ||
                    NewFollower.GetValue() ||
                    WorldRecordBeaten.GetValue() ||
                    NewDailyChallenge.GetValue() ||
                    DailyChallengeResults.GetValue();
        }
    }

    public void Initalise(PushNotificationStatusResponse status)
    {
        this.cachedStatus = status;

        MyLevelIsLevelOfTheWeek.SetValue(status.Notification_MyLevelIsLevelOfTheWeek);
        DailyChallengeResults.SetValue(status.Notification_DailyChallengeResults);
        FollowedPlayedLevel.SetValue(status.Notification_FollowedPlayedLevel);
        FollowedLikedLevel.SetValue(status.Notification_FollowedLikedLevel);
        FollowedPublishedLevel.SetValue(status.Notification_FollowedPublishedLevel);
        NewDailyChallenge.SetValue(status.Notification_NewDailyChallenge);
        NewFollower.SetValue(status.Notification_NewFollower);
        WorldRecordBeaten.SetValue(status.Notification_WorldRecordBeaten);
    }

    public override void Close(bool instant = false, System.Action onComplete = null)
    {   
        base.Close(instant, onComplete);
    }

    private bool ValuesHasChanged()
    {
        return  cachedStatus.Notification_DailyChallengeResults != DailyChallengeResults.GetValue() ||
                cachedStatus.Notification_MyLevelIsLevelOfTheWeek != MyLevelIsLevelOfTheWeek.GetValue() ||
                cachedStatus.Notification_NewFollower != NewFollower.GetValue() ||
                cachedStatus.Notification_FollowedPublishedLevel != FollowedPublishedLevel.GetValue() ||
                cachedStatus.Notification_WorldRecordBeaten != WorldRecordBeaten.GetValue() ||
                cachedStatus.Notification_NewDailyChallenge != NewDailyChallenge.GetValue() ||
                cachedStatus.Notification_FollowedPlayedLevel != FollowedPlayedLevel.GetValue() ||
                cachedStatus.Notification_FollowedLikedLevel != FollowedLikedLevel.GetValue();
    }

    public void OnClickClose()
    {
        base.Exit();
    }
}
