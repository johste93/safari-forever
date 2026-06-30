using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;
using System.Linq;

namespace SafariForever.Notifications
{
    public class NotificationAPI
    {
        public static int? cachedNotificationCount; 

        public delegate void NotificationCountUpdate(int count);
        public static NotificationCountUpdate On_NotificationCountUpdate;

        private const string getUnreadNotificationCountEndpoint = "notification/peek";
        private const string getNotificationsEndpoint = "notification/get-latest";
        private const string markAsReadEndpoint = "notification/mark-as-read";
        private const string getNotificationToggleStatusEndpoint = "notification/status";
        private const string toggleNotificationsEndpoint = "notification/toggle-notifications";

        public static void GetUnreadNotificationCount(System.Action<bool, int, Profile> onComplete)
        {
            bool failSilently = true;
            WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
            {
                if(!verified)
                {
                    onComplete?.Invoke(false, 0, null);
                    return;
                }
                
                string url = Globals.webConstants.GetHost() + getUnreadNotificationCountEndpoint;
                WebWorker worker = WebWorkerFactory.HireWorker();
                worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
                {   
                    if(!success)
                    {
                        onComplete?.Invoke(false, 0, null);
                        return;
                    }

                    if(int.TryParse(responseMsg, out int count))
                    {
                        cachedNotificationCount = count;
                    }
                    else
                    {
                        cachedNotificationCount = 0;
                    }
                    
                    On_NotificationCountUpdate?.Invoke(cachedNotificationCount.Value);
        
                    onComplete?.Invoke(true, count, profile);

                }, profile.token, failSilently);
            });
        }

        public static void GetNotifications(int fromIndex, System.Action<bool, NotificationFeedResponse> onComplete)
        {
            WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
            {
                if(!verified)
                     return;

                DialogCanvas.instance.ShowLoading();

                Dictionary<string, string> headers = new Dictionary<string, string> {
                    {"fromIndex",fromIndex.ToString()}
                };

                string url = Globals.webConstants.GetHost() + getNotificationsEndpoint;
                WebWorker worker = WebWorkerFactory.HireWorker();
                worker.Get(url, headers, (success, responseMsg, data)=>
                {   
                    DialogCanvas.instance.HideLoading();
                    
                    if(!success)
                    {
                        onComplete?.Invoke(false, new NotificationFeedResponse());
                        return;
                    }

                    NotificationFeedResponse notifications = JsonConvert.DeserializeObject<NotificationFeedResponse>(responseMsg);
                    onComplete?.Invoke(success, notifications);
                }, profile.token);
            });
        }

        public static void MarkAsRead(List<Notification> readNotifications)
        {
            readNotifications.RemoveAll(x => x.Read == true);
            if(readNotifications.Count == 0)
            {
                Debug.LogError("List is Empty!");
                return;
            }

            bool failSilently = true;
            WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
            {
                if(!verified)
                    return;
                
                readNotifications.ForEach(x => x.Read = true);
                List<string> readNotificationsIds = readNotifications.Select(x => x.NotificationId).ToList();

                SortedDictionary<string, object> body = new SortedDictionary<string, object>();
                body.Add("ReadNotificationsIds", readNotificationsIds);

                string url = Globals.webConstants.GetHost() + markAsReadEndpoint;
                WebWorker worker = WebWorkerFactory.HireWorker();
                worker.Patch(url, new Dictionary<string, string>(), body, (success, result)=>
                {   
                    if(!success)
                    {
                        Debug.LogError("Failed Silently: " + result);
                        return;
                    }

                    cachedNotificationCount = Mathf.Max(cachedNotificationCount.Value - readNotifications.Count, 0);
                    On_NotificationCountUpdate?.Invoke(cachedNotificationCount.HasValue ? cachedNotificationCount.Value : 0);
                }, profile.token, true);
            });
        }
    
        public static void GetNotificationToggleStatus(System.Action<bool, PushNotificationStatusResponse> onComplete, bool failSilently = false)
        {
            WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
            {
                if(!verified)
                {
                    onComplete(false, null);
                    return;
                }
                
                string url = Globals.webConstants.GetHost() + getNotificationToggleStatusEndpoint;
                WebWorker worker = WebWorkerFactory.HireWorker();
                worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
                {   
                    if(!success)
                    {
                        onComplete?.Invoke(false, null);
                        return;
                    }
                    
                    PushNotificationStatusResponse response = JsonConvert.DeserializeObject<PushNotificationStatusResponse>(responseMsg);

                    if(responseMsg == null)
                    {
                        new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, ()=>
                        {
                            onComplete?.Invoke(false, null);
                        }, true)
                        .Show(true);

                        return;
                    }

                    onComplete?.Invoke(success, response);
                }, profile.token, failSilently);
            });
        }

        public static void ToggleNotifications(bool followedPlayedLevel, bool followedLikedLevel, bool followedPublishedLevel, bool myLevelIsLevelOfTheWeek, bool newFollower, bool worldRecordBeaten, bool newDailyChallenge, bool dailyChallengeResults, System.Action<bool> onComplete)
        {
            bool failSilently = true;
            WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
            {
                if(!verified)
                    return;

                SortedDictionary<string, object> body = new SortedDictionary<string, object>();
                body.Add("Notification_FollowedPlayedLevel", followedPlayedLevel);
                body.Add("Notification_FollowedLikedLevel", followedLikedLevel);
                body.Add("Notification_MyLevelIsLevelOfTheWeek", myLevelIsLevelOfTheWeek);
                body.Add("Notification_FollowedPublishedLevel", followedPublishedLevel);
                body.Add("Notification_NewFollower", newFollower);
                body.Add("Notification_WorldRecordBeaten", worldRecordBeaten);
                body.Add("Notification_NewDailyChallenge", newDailyChallenge);
                body.Add("Notification_DailyChallengeResults", dailyChallengeResults);

                string url = Globals.webConstants.GetHost() + toggleNotificationsEndpoint;
                WebWorker worker = WebWorkerFactory.HireWorker();
                worker.Patch(url, new Dictionary<string, string>(), body, (success, result)=>
                {   
                    if(!success)
                    {
                        Debug.LogError("Failed Silently: " + result);
                        return;
                    }

                    onComplete?.Invoke(success);
                }, profile.token, true);
            });
        }
    }
}
