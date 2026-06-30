using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PushNotificationAPI
{	
	private const string registerFSMTokenEndpoint = "user/register-fcm-token";

	public static void RegisterFCMToken(string token, System.Action<bool> onComplete)
	{
		WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false);
                return;
            }

			if(profile == null)
			{
				onComplete?.Invoke(false);
				return;
			}
			
			SortedDictionary<string, object> body = new SortedDictionary<string, object>();
			body.Add("FCMToken", token);

			WebWorker worker = WebWorkerFactory.HireWorker();
			worker.Post(Globals.webConstants.GetHost() + registerFSMTokenEndpoint, new Dictionary<string, string>(), body, (success, result)=>
			{
				if(!success)
				{
					DialogCanvas.instance.HideLoading();
					onComplete?.Invoke(false);
					return;
				}

				onComplete?.Invoke(true);
			}, profile.token);
			
		});
	}
}
