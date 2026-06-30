using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BrowserAPI
{	

    private const string dailyChallengeEndpoint = "level/daily-challenge";
    private const string levelOfTheWeekEndpoint = "level/level-of-the-week";

	public static void FetchDailyChallenge(System.Action<bool, DailyChallengeResponse> onComplete,  bool failSilently = false)
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
			
			string url = Globals.webConstants.GetHost() + dailyChallengeEndpoint;

			WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {				
                if(!success)
                {
					DialogCanvas.instance.HideLoading();
					onComplete?.Invoke(false, null);             
                    return;
                }

                DailyChallengeResponse result = JsonConvert.DeserializeObject<DailyChallengeResponse>(responseMsg);

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

	public static void FetchLevelOfTheWeek(System.Action<bool, LevelOfTheWeekResponse> onComplete,  bool failSilently = false)
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
			
			string url = Globals.webConstants.GetHost() + levelOfTheWeekEndpoint;

			WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {				
                if(!success)
                {
					DialogCanvas.instance.HideLoading();
					onComplete?.Invoke(false, null);
                    return;
                }

                LevelOfTheWeekResponse result = JsonConvert.DeserializeObject<LevelOfTheWeekResponse>(responseMsg);

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

	public static void FetchNewLevels(int fromIndex, int amount, System.Action<bool, LevelFeedRespone> onComplete, bool failSilently = false)
	{
		FetchFeed("browse/new", fromIndex, amount, onComplete, failSilently);
	}

    public static void FetchBoostedLevels(int fromIndex, int amount, System.Action<bool, LevelFeedRespone> onComplete, bool failSilently = false)
	{
		FetchFeed("browse/boosted", fromIndex, amount, onComplete, failSilently);
	}

    public static void FetchBoostableLevels(int fromIndex, int amount, System.Action<bool, LevelFeedRespone> onComplete, bool failSilently = false)
	{
		FetchFeed("browse/boostable", fromIndex, amount, onComplete, failSilently);
	}

	public static void FetchTrendingLevels(int fromIndex, int amount, System.Action<bool, LevelFeedRespone> onComplete, bool failSilently = false)
	{
		FetchFeed("browse/trending", fromIndex, amount, onComplete, failSilently);
	}

	public static void FetchPopularLevels(int fromIndex, int amount, System.Action<bool, LevelFeedRespone> onComplete, bool failSilently = false)
	{
		FetchFeed("browse/popular", fromIndex, amount, onComplete, failSilently);
	}

    public static void FetchPreviousDailyChallenges(int fromIndex, int amount, System.Action<bool, LevelFeedRespone> onComplete, bool failSilently = false)
	{
		FetchFeed("browse/previous-daily-challenges", fromIndex, amount, onComplete, failSilently);
	}

    public static void FetchPreviousLevelsOfTheWeek(int fromIndex, int amount, System.Action<bool, LevelFeedRespone> onComplete, bool failSilently = false)
	{
		FetchFeed("browse/previous-levels-of-the-week", fromIndex, amount, onComplete, failSilently);
	}

	private static void FetchFeed(string endpoint, int fromIndex, int amount, System.Action<bool, LevelFeedRespone> onComplete, bool failSilently = false)
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

            WebWorker worker = WebWorkerFactory.HireWorker();

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
			body.Add("FromIndex", fromIndex);
            body.Add("Amount", amount);

            string url = Globals.webConstants.GetHost() + endpoint;
            worker.Post(url, new Dictionary<string, string>(), body, (success, response)=>
            {				
                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                LevelFeedRespone result = JsonConvert.DeserializeObject<LevelFeedRespone>(response);

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
}
