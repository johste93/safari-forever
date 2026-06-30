using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleNotificationsRequest
{
    public bool Notification_FollowedPlayedLevel { get; set; }
    public bool Notification_FollowedLikedLevel { get; set; }
    public bool Notification_FollowedPublishedLevel { get; set; }
    public bool Notification_MyLevelIsLevelOfTheWeek { get; set; }
    public bool Notification_NewFollower { get; set; }
    public bool Notification_WorldRecordBeaten { get; set; }
    public bool Notification_NewDailyChallenge { get; set; }
    public bool Notification_DailyChallengeResults { get; set; }
}
