using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LevelAPI
{
    private const string fetchUserLevelsEndpoint = "level/by-user";
    private const string downloadEndpoint = "level/{0}/download";
    private const string getImageEndpoint = "level/{0}/img";
    private const string countPlayEndpoint = "level/{0}/play";
    private const string countDeathEndpoint = "level/{0}/death";
    private const string logLevelCompleteEndpoint = "level/{0}/complete";
    private const string loadUserStatsEndpoint = "level/{0}/user-stats";
    private const string likeLevelEndpoint = "level/{0}/like";
    private const string dislikeLevelEndpoint = "level/{0}/dislike";
    private const string clearLevelOpinionEndpoint = "level/{0}/clear-opinion";
    private const string pullStatsEndpoint = "level/{0}/stats";
    private const string uploadLevelEndpoint = "level/upload";
    private const string verifyUploadEndpoint = "level/upload/verify/{0}";
    
    public static void FetchUserLevels(string userId, int count, int fromIndex, System.Action<bool, LevelsByUserResponse> onComplete, bool failSilently = false)
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

		    SortedDictionary<string, object> body = new SortedDictionary<string, object>();
			body.Add("UserId", userId);
			body.Add("Count", count);
			body.Add("FromIndex", fromIndex);

			string url = Globals.webConstants.GetHost() + fetchUserLevelsEndpoint;
			WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(url, new Dictionary<string, string>(), body, (success, responseMsg)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                LevelsByUserResponse result = JsonConvert.DeserializeObject<LevelsByUserResponse>(responseMsg);

                if(result == null)
                {
                    new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, ()=>
                        {
                            onComplete?.Invoke(false, null);
                        }, true)
                        .Show(true);

                    return;
                }

                onComplete?.Invoke(true, result);

            }, profile.token, failSilently);
        });
    }

    public static void Download(string levelId, System.Action<Level> onComplete)
    {
        WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
        {
            if(!verified)
                return;

            DialogCanvas.instance.ShowLoading();
            
            string url = Globals.webConstants.GetHost() + string.Format(downloadEndpoint, levelId);
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                DialogCanvas.instance.HideLoading();
                if(!success)
                {
                    onComplete.Invoke(null);
                    return;
                }

                LevelDownloadResponse levelDownloadDTO = JsonConvert.DeserializeObject<LevelDownloadResponse>(responseMsg);
                if(levelDownloadDTO == null)
                {
                    Debug.LogError(responseMsg);
                    new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                            onComplete.Invoke(null);
                        }, true)
                        .Show(true);

                    return;
                }
                onComplete(levelDownloadDTO.ToLevel());

            }, profile.token);
        });
    }

    public static void DownloadNoVerify(string levelId, System.Action<Level> onComplete)
    {
        DialogCanvas.instance.ShowLoading();

        string url = Globals.webConstants.GetHost() + string.Format(downloadEndpoint, levelId);
        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
        {
            DialogCanvas.instance.HideLoading();
            if(!success)
            {
                onComplete.Invoke(null);
                return;
            }

            LevelDownloadResponse levelDownloadDTO = JsonConvert.DeserializeObject<LevelDownloadResponse>(responseMsg);
            if(levelDownloadDTO == null)
            {
                Debug.LogError(responseMsg);
                new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                    .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                        onComplete.Invoke(null);
                    }, true)
                    .Show(true);

                return;
            }
            onComplete(levelDownloadDTO.ToLevel());

        });
    }
    
    public static void GetImage(string levelId, System.Action<bool, byte[]> onComplete)
	{
		string url = Globals.webConstants.GetHost() + string.Format(getImageEndpoint, levelId);
		WebWorker worker = WebWorkerFactory.HireWorker();
		worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
		{
			if(!success || data == null || data.Length == 0)
			{
                onComplete?.Invoke(false, null);
				return;
			}

			onComplete?.Invoke(true, data);
		});
	}

    public static void CountPlay(string levelId, string creatorUserId, System.Action<bool> onComplete)
    {
        bool failSilently = true;
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false);
                return;
            }

            if(creatorUserId == profile.userId)
            {
                onComplete?.Invoke(false);
                return;
            }

            string url = Globals.webConstants.GetHost() + string.Format(countPlayEndpoint, levelId);
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                onComplete?.Invoke(true);
            }, profile.token, failSilently);
        });
    }

    public static void CountDeath(string levelId, string creatorUserId, System.Action<bool> onComplete)
    {
        bool failSilently = true;
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false);
                return;
            }

            if(creatorUserId == profile.userId)
            {
                onComplete?.Invoke(false);
                return;
            }
                
            string url = Globals.webConstants.GetHost() + string.Format(countDeathEndpoint, levelId);
            WebWorker worker = WebWorkerFactory.HireWorker(); 
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                onComplete?.Invoke(true);
            }, profile.token, failSilently);
        });
    }

    public static void LogLevelComplete(string levelId, string creatorUserId, double time, int deaths, int jumps, System.Action<bool> onComplete)
    {
        bool failSilently = true;
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile) =>
        {
            if (!verified)
            {
                onComplete?.Invoke(false);
                return;
            }

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
            body.Add("Deaths", deaths);
            body.Add("Jumps", jumps);
            body.Add("Seconds", (int) time);
            body.Add("Milliseconds", (int)((time - ((int)time)) * 100));

            string url = Globals.webConstants.GetHost() + string.Format(logLevelCompleteEndpoint, levelId);
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(url, new Dictionary<string, string>(), body, (success, result) =>
            {
                if (!success)
                {
                    onComplete.Invoke(false);
                    return;
                }


                onComplete?.Invoke(true);
            }, profile.token, failSilently);
        });
    }

    public static void LoadUserStats(string levelId, System.Action<bool, LevelUserStats> onComplete)
    {
        bool failSilently = true;
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            string url = Globals.webConstants.GetHost() + string.Format(loadUserStatsEndpoint, levelId);
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                if(string.IsNullOrWhiteSpace(responseMsg))
                {
                    onComplete?.Invoke(true, new LevelUserStats(){
                        LevelId = levelId
                    });
                    return;
                }    

                LevelUserStats levelUserStats = JsonConvert.DeserializeObject<LevelUserStats>(responseMsg);

                if(levelUserStats == null)
                {
                    Debug.LogError("Unable to deserialize user level stats");
                    onComplete(false, null);
                    return;
                }

                onComplete?.Invoke(true, levelUserStats);
            }, profile.token, failSilently);
        });
    }

    public static void LikeLevel(string levelId, string creatorUserId, System.Action<bool> onComplete)
    {
        bool failSilently = true;
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false);
                return;
            }

            if(creatorUserId == profile.userId)
            {
                onComplete?.Invoke(false);
                return;
            }

            string url = Globals.webConstants.GetHost() + string.Format(likeLevelEndpoint, levelId);
            WebWorker worker = WebWorkerFactory.HireWorker(); 
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                onComplete?.Invoke(true);
            }, profile.token, failSilently);
        });
    }

    public static void DislikeLevel(string levelId, string creatorUserId, System.Action<bool> onComplete)
    {
        bool failSilently = true;
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false);
                return;
            }

            if(creatorUserId == profile.userId)
            {
                onComplete?.Invoke(false);
                return;
            }
                
            string url = Globals.webConstants.GetHost() + string.Format(dislikeLevelEndpoint, levelId);
            WebWorker worker = WebWorkerFactory.HireWorker(); 
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                onComplete?.Invoke(true);
            }, profile.token, failSilently);
        });
    }

    public static void ClearLevelOpinion(string levelId, string creatorUserId, System.Action<bool> onComplete)
    {
        bool failSilently = true;
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false);
                return;
            }

            if(creatorUserId == profile.userId)
            {
                onComplete?.Invoke(false);
                return;
            }
                
            string url = Globals.webConstants.GetHost() + string.Format(clearLevelOpinionEndpoint, levelId);
            WebWorker worker = WebWorkerFactory.HireWorker(); 
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                onComplete?.Invoke(true);
            }, profile.token, failSilently);
        });
    }

    public static void PullStats(string levelId, System.Action<bool, LevelStatsResponse> onComplete)
    {
        bool failSilently = true;

        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false, null);
                return;
            }
            
            string url = Globals.webConstants.GetHost() + string.Format(pullStatsEndpoint, levelId);
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data) =>
            {
                if (!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                LevelStatsResponse levelStatsResponse = JsonConvert.DeserializeObject<LevelStatsResponse>(responseMsg);

                if(levelStatsResponse == null)
                {
                    Debug.LogError(responseMsg);
                    new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                            onComplete?.Invoke(false, null);
                        }, true)
                        .Show(true);

                    return;
                }

                onComplete?.Invoke(true, levelStatsResponse);
            }, null, failSilently);
        });
    }

    public static void UploadLevel(string levelName, JObject serializedLevel, byte[] largeThumbnail,  byte[] smallThumbnail, System.Action onReady, System.Action<bool, string> onComplete)
    {    
        WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false, string.Empty);
                return;
            }

            onReady?.Invoke();

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
            body.Add("Name", levelName);
            body.Add("GameVersion", Application.version);
            body.Add("SerializedLevel", serializedLevel.ToString());
            body.Add("Thumbnail", System.Convert.ToBase64String(largeThumbnail));
            body.Add("MiniThumbnail", System.Convert.ToBase64String(smallThumbnail));

            string url = Globals.webConstants.GetHost() + uploadLevelEndpoint;
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(url, new Dictionary<string, string>(), body, (success, result) =>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }
                else
                {
                    JObject jResult = JsonConvert.DeserializeObject<JObject>(result);
                    
                    JToken levelIdToken;
                    if(!jResult.TryGetValue("levelId", out levelIdToken ))
                    {
                        new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                            .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                            .Show(true);

                        onComplete?.Invoke(false, null);
                        return;
                    }

                    string levelId = levelIdToken.Value<string>();

                    onComplete?.Invoke(!string.IsNullOrWhiteSpace(levelId), levelId);
                }

            }, profile.token, false, 60);
        });
    }

    public static void VerifyUpload(string levelId, System.Action<bool, string> onComplete)
    {
        WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false, string.Empty);
                return;
            }

            string url = Globals.webConstants.GetHost() + string.Format(verifyUploadEndpoint, levelId);
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, string.Empty);
                    return;
                }
                else
                {
                    JObject jResult = JsonConvert.DeserializeObject<JObject>(responseMsg);
                    
                    JToken shareUrl;
                    if(!jResult.TryGetValue("shareUrl", out shareUrl ))
                    {
                        new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                            .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                            .Show(true);

                        onComplete?.Invoke(false, string.Empty);
                        return;
                    }

                    string shareUrlString = shareUrl.Value<string>();

                    onComplete?.Invoke(!string.IsNullOrWhiteSpace(shareUrlString), shareUrlString);
                }
            },profile.token, false, 60);
        });
    }
}
