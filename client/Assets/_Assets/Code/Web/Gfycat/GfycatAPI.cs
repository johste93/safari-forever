using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class GfycatAPI
{
    private const string requestAccessTokenUrl = "https://api.gfycat.com/v1/oauth/token";
    private const string requestUploadKeyUrl = "https://api.gfycat.com/v1/gfycats";
    private const string uploadFileUrl = "https://filedrop.gfycat.com";
    private const string requestUploadStatusUrl = "https://api.gfycat.com/v1/gfycats/fetch/status/{0}";
    private const string getGfycatUrl = "https://api.gfycat.com/v1/gfycats/{0}";

    public static void RequestAccessToken(System.Action<bool, GfycatAccessTokenResponse> onComplete)
    {
        SortedDictionary<string, object> body = new SortedDictionary<string, object>();
        body.Add("grant_type", "client_credentials");
        body.Add("client_id", "");
        body.Add("client_secret", "");

        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.Post(requestAccessTokenUrl, new Dictionary<string, string>(), body, (success, responseMsg) =>
        {
            if (!success)
            {
                Debug.LogError("Unable to request access token");
                onComplete?.Invoke(false, null);
                return;
            }

            GfycatAccessTokenResponse gfycatAccessTokenResponse = JsonConvert.DeserializeObject<GfycatAccessTokenResponse>(responseMsg);

            if(gfycatAccessTokenResponse == null)
            {
                Debug.LogError(responseMsg);
                new Dialog(TranslationKey.Generic_UploadError, TranslationKey.Generic_InvalidResponse)
                    .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                        onComplete?.Invoke(false, null);
                    }, true)
                    .Show(true);

                return;
            }

            onComplete?.Invoke(true, gfycatAccessTokenResponse);
        });
    }

    public static void RequestUploadKey(string accessToken, System.Action<bool, GfycatUploadKeyResponse> onComplete)
    {
        SortedDictionary<string, object> body = new SortedDictionary<string, object>();
        body.Add("title", "Safari Forever Replay");
        body.Add("description", "https://safariforever.com");
        body.Add("tags", new string[]{"Safari Forever", "SafariForever", "Game"});
        body.Add("noMd5",true);
        body.Add("nsfw",0);
        body.Add("private",false);

        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.Post(requestUploadKeyUrl, new Dictionary<string, string>(), body, (success, responseMsg) =>
        {
            if (!success)
            {
                Debug.LogError("Unable to request upload key");
                onComplete?.Invoke(false, null);
                return;
            }

            GfycatUploadKeyResponse gfycatUploadKeyResponse = JsonConvert.DeserializeObject<GfycatUploadKeyResponse>(responseMsg);

            if(gfycatUploadKeyResponse == null)
            {
                Debug.LogError(responseMsg);
                new Dialog(TranslationKey.Generic_UploadError, TranslationKey.Generic_InvalidResponse)
                    .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                        onComplete?.Invoke(false, null);
                    }, true)
                    .Show(true);

                return;
            }

            onComplete?.Invoke(true, gfycatUploadKeyResponse);
        }, accessToken);
    }

    public static void UploadFile(string uploadKey, byte[] gifBytes, System.Action<bool> onComplete)
    {
        SortedDictionary<string, object> body = new SortedDictionary<string, object>();
        body.Add("key", uploadKey);
        body.Add("gif", Convert.ToBase64String(gifBytes));

        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.Post(uploadFileUrl, new Dictionary<string, string>(), body, (success, responseMsg) =>
        {
            if (!success)
            {
                Debug.LogError("Unable to upload gif");
                onComplete?.Invoke(false);
                return;
            }

            onComplete?.Invoke(success);
        });
    }

    public static void RequestUploadStatus(string uploadKey, System.Action<bool, GfycatStatusResponse> onComplete)
    {
        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.Get(string.Format(requestUploadStatusUrl, uploadKey), new Dictionary<string, string>(), (success, responseMsg, data) =>
        {
            if (!success)
            {
                Debug.LogError("Unable to fetch upload status");
                onComplete?.Invoke(false, null);
                return;
            }

            GfycatStatusResponse gfycatStatusResponse = JsonConvert.DeserializeObject<GfycatStatusResponse>(responseMsg);

            if(gfycatStatusResponse == null)
            {
                Debug.LogError(responseMsg);
                new Dialog(TranslationKey.Generic_UploadError, TranslationKey.Generic_InvalidResponse)
                    .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                        onComplete?.Invoke(false, null);
                    }, true)
                    .Show(true);

                return;
            }

            onComplete?.Invoke(true, gfycatStatusResponse);
        });
    }

    public static void GetGfycat(string uploadKey, System.Action<bool, GfycatGetResponse> onComplete)
    {
        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.Get(string.Format(getGfycatUrl, uploadKey), new Dictionary<string, string>(), (success, responseMsg, data) =>
        {
            if (!success)
            {
                Debug.LogError("Unable to fetch gfycat");
                onComplete?.Invoke(false, null);
                return;
            }

            GfycatGetResponse gfycatGetResponse = JsonConvert.DeserializeObject<GfycatGetResponse>(responseMsg);

            if(gfycatGetResponse == null)
            {
                Debug.LogError(responseMsg);
                new Dialog(TranslationKey.Generic_UploadError, TranslationKey.Generic_InvalidResponse)
                    .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                        onComplete?.Invoke(false, null);
                    }, true)
                    .Show(true);

                return;
            }

            onComplete?.Invoke(true, gfycatGetResponse);
        });
    }
}
