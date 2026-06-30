using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class FollowerAPI
{
    private const string followUserEndpoint = "user/follow";
    private const string unfollowUserEndpont = "user/unfollow";
    private const string fetchFollowingsEndpoint = "user/followings";

    public static void FollowUser(string userId, System.Action<bool> onComplete)
    {
        DialogCanvas.instance.ShowLoading();

        WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
        {
            if(!verified)
            {
                DialogCanvas.instance.HideLoading();
                onComplete?.Invoke(false);
                return;   
            }

            if(profile == null)
            {
                DialogCanvas.instance.HideLoading();
                onComplete?.Invoke(false);
                return;
            }

            WebWorker worker = WebWorkerFactory.HireWorker();

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
            body.Add("UserToFollow", userId);

            string url = Globals.webConstants.GetHost() + followUserEndpoint;
            worker.Post(url, new Dictionary<string, string>(), body, (success, responseMsg)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                onComplete?.Invoke(true);
            }, profile.token);
        });
    }

    public static void UnfollowUser(string userId, System.Action<bool> onComplete)
    {
        DialogCanvas.instance.ShowLoading();

        WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
        {
            if(!verified)
            {
                DialogCanvas.instance.HideLoading();
                onComplete?.Invoke(false);
                return; 
            }

            if(profile == null)
            {
                DialogCanvas.instance.HideLoading();
                onComplete?.Invoke(false);
                return;
            }

            WebWorker worker = WebWorkerFactory.HireWorker();

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
            body.Add("UserToUnfollow", userId);

            string url = Globals.webConstants.GetHost() + unfollowUserEndpont;
            worker.Post(url, new Dictionary<string, string>(), body, (success, responseMsg)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                onComplete?.Invoke(true);
            }, profile.token);
        });
    }

    public static void FetchFollowings(int fromIndex, System.Action<bool, FollowedUsersResponse> onComplete)
    {
        DialogCanvas.instance.ShowLoading();

        WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
        {
            if(!verified)
            {
                DialogCanvas.instance.HideLoading();
                onComplete?.Invoke(false, null);
                return;
            }

            if(profile == null)
            {
                DialogCanvas.instance.HideLoading();
                onComplete?.Invoke(false, null);
                return;
            }

            WebWorker worker = WebWorkerFactory.HireWorker();

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
            body.Add("Count", 10);
            body.Add("FromIndex", fromIndex);

            string url = Globals.webConstants.GetHost() + fetchFollowingsEndpoint;
            worker.Post(url, new Dictionary<string, string>(), body, (success, result)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                FollowedUsersResponse response = JsonConvert.DeserializeObject<FollowedUsersResponse>(result);
                if(response == null)
                {
                    new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                        .Show(true);

                    onComplete?.Invoke(false, null);
                    return;
                }

                onComplete?.Invoke(true, response);

            }, profile.token);
        });
    }

    public static void FetchFollowerCount(string userId, System.Action<bool, int> onComplete)
    {
        DialogCanvas.instance.ShowLoading();

        WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
        {
            if(!verified)
            {
                DialogCanvas.instance.HideLoading();
                onComplete?.Invoke(false, -1);
                return;   
            }

            if(profile == null)
            {
                DialogCanvas.instance.HideLoading();
                onComplete?.Invoke(false, -1);
                return;
            }


            WebWorker worker = WebWorkerFactory.HireWorker();

            string url = Globals.webConstants.GetHost() + "user/follower-count";
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                {
                    onComplete?.Invoke(false, -1);
                    return;
                }

                JObject jResult = JsonConvert.DeserializeObject<JObject>(responseMsg);
                
                JToken count;
                if(!jResult.TryGetValue("count", out count))
                {
                    new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                        .Show(true);

                    onComplete(false, -1);
                    return;
                }

                onComplete(true, count.Value<int>());

            }, profile.token);
        });
    }
}
