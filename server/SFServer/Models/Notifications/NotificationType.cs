using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Notifications
{
    public enum NotificationType
    {
        DontSendPushNotification,
        FollowedPlayedLevel,
        FollowedLikedLevel,
        MyLevelIsLevelOfTheWeek,
        NewFollower,
        WorldRecordBeaten,
        NewDailyChallenge,
        DailyChallengeResults,
        FollowedPublishedLevel,
        News
    }
}