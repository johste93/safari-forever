using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SearchAPI
{
    private const string userSearchEndpoint = "search/user";
    private const string levelSearchEndpoint = "search/level";

    public static void SearchLevel(string query, System.Action<bool, LevelSearchResponse> onComplete, bool failSilently = false)
    {
        DialogCanvas.instance.ShowLoading();
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
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
            worker.Get(Globals.webConstants.GetHost() + levelSearchEndpoint + $"/{query}", new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                DialogCanvas.instance.HideLoading();
                if(!success)
                {
					onComplete?.Invoke(false, null);
                    return;
                }

                LevelSearchResponse result = JsonConvert.DeserializeObject<LevelSearchResponse>(responseMsg);

                if(result == null)
                {
					if(!failSilently)
					{
						new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
							.AddNeutralButton(TranslationKey.Generic_Ok, ()=>
							{
								onComplete?.Invoke(false, null);
							}, true)
							.Show(true);
					}
					else
						onComplete?.Invoke(false, null);

                    return;
                }

                onComplete?.Invoke(true, result);

            }, profile.token, failSilently);
        });
    }

    public static void SearchUser(string query, System.Action<bool, PlayerSearchResponse> onComplete, bool failSilently = false)
    {
        DialogCanvas.instance.ShowLoading();
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            if(!verified)
            {
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
            worker.Get(Globals.webConstants.GetHost() + userSearchEndpoint + $"/{query}", new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                DialogCanvas.instance.HideLoading();
                if(!success)
                {
					onComplete?.Invoke(false, null);
                    return;
                }

                PlayerSearchResponse result = JsonConvert.DeserializeObject<PlayerSearchResponse>(responseMsg);

                if(result == null)
                {
					if(!failSilently)
					{
						new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
							.AddNeutralButton(TranslationKey.Generic_Ok, ()=>
							{
								onComplete?.Invoke(false, null);
							}, true)
							.Show(true);
					}
					else
						onComplete?.Invoke(false, null);

                    return;
                }

                onComplete?.Invoke(true, result);

            }, profile.token, failSilently);
        });
    }
}
