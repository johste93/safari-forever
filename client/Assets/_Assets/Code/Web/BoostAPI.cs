using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BoostAPI
{
    private const string rankEndpoint = "boost/rank";
    private const string rankingEndpoint = "boost/ranking";
    private const string boostEndpoint = "boost/boost";

    public static void GetRank(int amountInvested, long createdOn, System.Action<bool, RankResponse> onComplete, bool failSilently = false)
    {
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            if(profile == null)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            string url = Globals.webConstants.GetHost() + rankEndpoint;

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
            body.Add("AmountInvested", amountInvested);
			body.Add("CreatedOn", createdOn);

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(url, new Dictionary<string, string>(), body, (success, responseMsg)=>
            {
                if(!success)
                {
					onComplete?.Invoke(false, null);             
                    return;
                }

                RankResponse result = JsonConvert.DeserializeObject<RankResponse>(responseMsg);

                if(result == null)
                {
					if(!failSilently)
					{
						onComplete?.Invoke(false, null);
						new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
							.AddNeutralButton(TranslationKey.Generic_Ok, null, true)
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

    public static void FetchRankings(System.Action<bool, NewRankingsResponse> onComplete, bool failSilently = false)
    {
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            if(profile == null)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            string url = Globals.webConstants.GetHost() + rankingEndpoint;

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
					onComplete?.Invoke(false, null);             
                    return;
                }

                NewRankingsResponse result = JsonConvert.DeserializeObject<NewRankingsResponse>(responseMsg);

                if(result == null)
                {
					if(!failSilently)
					{
						onComplete?.Invoke(false, null);
						new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
							.AddNeutralButton(TranslationKey.Generic_Ok, null, true)
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

    public static void BoostLevel(string levelId, int amount, System.Action<bool> onComplete, bool failSilently = false)
    {
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
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

            string url = Globals.webConstants.GetHost() + boostEndpoint;

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
            body.Add("LevelId", levelId);
			body.Add("Amount", amount);

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(url, new Dictionary<string, string>(), body, (success, responseMsg)=>
            {
                onComplete?.Invoke(success);
            }, profile.token, failSilently);
        });
    }
}
