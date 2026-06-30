using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class HighscoreAPI
{
    private const string getHighscoreEndpoint = "highscore/campaign/{0}/{1}/best";
    private const string uploadScoreEndpoint = "highscore/campaign/{0}/{1}/upload";

    public static void GetHighscore(int worldIndex, int levelIndex, System.Action<bool, float> onComplete)
    {
        bool failSilently = true;
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false, -1);
                return;   
            }

            WebWorker worker = WebWorkerFactory.HireWorker();
            string url = Globals.webConstants.GetHost() + string.Format(getHighscoreEndpoint, worldIndex, levelIndex);
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, -1);
                    return;
                }

                if(!float.TryParse(responseMsg, NumberStyles.Any, CultureInfo.InvariantCulture, out float score))
                {
                    Debug.LogError("Failed Silently: Unable to parse score");
                    onComplete?.Invoke(false, -1);
                    return;
                }

                onComplete?.Invoke(true, score);
            }, profile.token, failSilently);
        });
    }

    public static void UploadScore(Highscore time, int world, int index)
    {   
        if(SaveManager.currentSave.gameSpeed > (0.9f + Mathf.Epsilon))
        {
            Debug.LogWarning("Unable to upload score when gamespeed is more than 100%");
            return;
        }

        bool failSilently = true;
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
                return;


            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
			body.Add("Seconds", time.Seconds);
            body.Add("Milliseconds", time.Milliseconds);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("ClientId", Client.clientId);
            headers.Add("ClientSecret", Client.GetClientVerificationSecret());

            string url = Globals.webConstants.GetHost() + string.Format(uploadScoreEndpoint, world, index);
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(url, headers, body, (success, responseMsg)=>
            {
                if(!success)
                {
                    Debug.LogError("Unable to upload Highscore");
                    return;
                }
            }, profile.token, failSilently );
        });
    }
/*
    public static void UploadScore(double time, string levelID)
    {   
        if(SaveManager.currentSave.gameSpeed < 1f)
        {
            Debug.LogError("Unable to upload score when gamespeed is less than 100%");
            return;
        }

        bool failSilently = true;
        WebRequestMiddleware.FetchAndVerifyProfile(failSilently, (verified, profile)=>
        {
            if(!verified)
                return;

            JObject body = new JObject();
			body.Add("Time", time);
            string jsonBody = JsonConvert.SerializeObject(body);

            string url = Globals.webConstants.GetHost() + $"level/{levelID}/time";
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(url, new Dictionary<string, string>(), jsonBody, (success, responseMsg)=>
            {
                if(!success)
                {
                    Debug.LogError("Unable to upload Highscore");
                    return;
                }
            }, profile.token, failSilently );
        });
    }
*/
}
