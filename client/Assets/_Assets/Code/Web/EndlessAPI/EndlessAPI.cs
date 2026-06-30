using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EndlessAPI
{
    private const string fetchChallengeEndpoint = "endless/challenge";
    private const string skipChallengeEndpoint = "endless/skip";
    private const string leaderboardEndpoint = "endless/leaderboard/{0}";
    private const string fetchRankEndpoint = "endless/rank";

    public static void FetchChallenge(System.Action<bool, EndlessChallengeResponse> onComplete)
    {
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            string url = Globals.webConstants.GetHost() + fetchChallengeEndpoint;

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data) =>
            {
                if (!success)
                {
                    Debug.LogError("Unable to get endless challenge");
                    onComplete?.Invoke(false, null);
                    return;
                }

                EndlessChallengeResponse response = JsonConvert.DeserializeObject<EndlessChallengeResponse>(responseMsg);

                if(response == null)
                {
                    Debug.LogError(responseMsg);
                    onComplete?.Invoke(false, null);
                    new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                        .Show(true);

                    return;
                }

                onComplete?.Invoke(true, response);
            }, profile.token);
        });
    }

    public static void SkipChallenge(System.Action<bool, EndlessChallengeResponse> onComplete)
    {
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            string url = Globals.webConstants.GetHost() + skipChallengeEndpoint;

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data) =>
            {
                if (!success)
                {
                    Debug.LogError("Unable to skip challenge");
                    onComplete?.Invoke(false, null);
                    return;
                }

                EndlessChallengeResponse response = JsonConvert.DeserializeObject<EndlessChallengeResponse>(responseMsg);

                if(response == null)
                {
                    Debug.LogError(responseMsg);
                    onComplete?.Invoke(false, null);
                    new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                        .Show(true);

                    return;
                }

                onComplete?.Invoke(true, response);
            }, profile.token);
        });
    }

    public static void FetchLeaderboard(int page, System.Action<bool, FetchLeaderboardResponse> onComplete)
    {
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            string url = Globals.webConstants.GetHost() +  string.Format(leaderboardEndpoint, page);
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data) =>
            {
                if (!success)
                {
                    Debug.LogError("Unable fetch leaderboard");
                    onComplete?.Invoke(false, null);
                    return;
                }

                FetchLeaderboardResponse response = JsonConvert.DeserializeObject<FetchLeaderboardResponse>(responseMsg);

                if(response == null)
                {
                    Debug.LogError(responseMsg);
                    onComplete?.Invoke(false, null);
                    new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                        .Show(true);

                    return;
                }

                onComplete?.Invoke(true, response);
            }, profile.token);
        });
    }

    public static void FetchRank(System.Action<bool, RankResponse> onComplete)
    {
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            string url = Globals.webConstants.GetHost() + fetchRankEndpoint;

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data) =>
            {
                if (!success)
                {
                    Debug.LogError("Unable to fetch rank");
                    onComplete?.Invoke(false, null);
                    return;
                }

                RankResponse response = JsonConvert.DeserializeObject<RankResponse>(responseMsg);

                if(response == null)
                {
                    Debug.LogError(responseMsg);
                    onComplete?.Invoke(false, null);
                    new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                        .Show(true);

                    return;
                }

                onComplete?.Invoke(true, response);
            }, profile.token);
        });
    }
}
