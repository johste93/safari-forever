using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFServer.Models.Requests.NotificationRequests
{
    public class ToggleNotificationsRequest
    {
        public bool? Notification_FollowedLikedLevel { get; set; }
        public bool? Notification_FollowedPlayedLevel { get; set; }
        public bool? Notification_FollowedPublishedLevel { get; set; }
        public bool? Notification_MyLevelIsLevelOfTheWeek { get; set; }
        public bool? Notification_NewFollower { get; set; }
        public bool? Notification_WorldRecordBeaten { get; set; }
        public bool? Notification_NewDailyChallenge { get; set; }
        public bool? Notification_DailyChallengeResults { get; set; }
    }
}
