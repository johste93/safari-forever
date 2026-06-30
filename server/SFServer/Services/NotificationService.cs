using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FCM.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SFServer.Contexts;
using SFServer.Models.DB;
using SFServer.Models.Notifications;

namespace SFServer.Services
{
    public class NotificationService
    {
        private PostgreSqlContext _db;
        private IConfiguration _config;

        public NotificationService(PostgreSqlContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task CreateNotification(string reciverUserId, string title, string body, NotificationType type, string deeplinkUrl = "", List<NotificationLink> links = null)
        {
            _db.Notifications.Add(new Models.Notifications.Notification(reciverUserId, title, body, type, deeplinkUrl, links));
            await SendPushNotification(reciverUserId, title, body, type, deeplinkUrl);
        }

        private async Task<bool> WantsPushNotification(string reciverUserId, NotificationType type)
        {
            if (type == NotificationType.DontSendPushNotification)
                return false;

            User reciver = await _db.Users.FindAsync(reciverUserId);

            if (reciver == null)
                return false;

            switch(type)
            {
                case NotificationType.DailyChallengeResults:
                    return reciver.Notification_DailyChallengeResults;
                case NotificationType.FollowedPlayedLevel:
                    return reciver.Notification_FollowedPlayedLevel;
                case NotificationType.FollowedLikedLevel:
                    return reciver.Notification_FollowedLikedLevel;
                case NotificationType.MyLevelIsLevelOfTheWeek:
                    return reciver.Notification_MyLevelIsLevelOfTheWeek;
                case NotificationType.NewDailyChallenge:
                    return reciver.Notification_NewDailyChallenge;
                case NotificationType.NewFollower:
                    return reciver.Notification_NewFollower;
                case NotificationType.WorldRecordBeaten:
                    return reciver.Notification_WorldRecordBeaten;
                case NotificationType.FollowedPublishedLevel:
                    return reciver.Notification_FollowedPublishedLevel;
                default:
                case NotificationType.DontSendPushNotification:
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationId">ID generated when device is registered in FCM</param>
        public async Task SendPushNotification(string reciverUserId, string title, string body, NotificationType type, string deeplinkUrl = "")
        {
            if (!await WantsPushNotification(reciverUserId, type))
                return;

            var registrationIds = await _db.Clients.Select(x => new { x.UserId, x.FCMToken }).Where(x => x.UserId == reciverUserId && x.FCMToken != null).Select(x => x.FCMToken).ToListAsync();

            if (registrationIds == null || registrationIds.Count == 0)
                return;

            Console.WriteLine(DateTime.Now.ToString() + $": Sending push notification of type: {type} to {reciverUserId}");
            foreach (string s in registrationIds)
                Console.WriteLine(s);

            //You can get the server Key by accessing the url/
            //https://console.firebase.google.com/project/MY_PROJECT/settings/cloudmessaging";
            using (var sender = new Sender(_config["FCM_SECRET_KEY"]))
            {
                var message = new Message
                {
                    RegistrationIds = registrationIds,
                    Notification = new FCM.Net.Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Data = new Dictionary<string, string>()
                    {
                        {"deeplink", deeplinkUrl}
                    }
                };
                var result = await sender.SendAsync(message);
                Console.WriteLine($"Success: {result.MessageResponse.Success}");
            }
        }
    }
}
