using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GifAPI
{
    private const string uploadEndpoint = "gif/uploadToGiphy";

    public static void Upload(byte[] gifBytes, System.Action<bool, GifUploadResponse> onComplete)
    {
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
			body.Add("Base64", Convert.ToBase64String(gifBytes));

            string url = Globals.webConstants.GetHost() + uploadEndpoint;
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(url, new Dictionary<string, string>(), body, (success, responseMsg) =>
            {
                if (!success)
                {
                    Debug.LogError("Unable to upload gif");
                    onComplete?.Invoke(false, null);
                    return;
                }

                GifUploadResponse giphyResponse = JsonConvert.DeserializeObject<GifUploadResponse>(responseMsg);

                if(giphyResponse == null)
                {
                    Debug.LogError(responseMsg);
                    new Dialog(TranslationKey.Generic_UploadError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                            onComplete?.Invoke(false, null);
                        }, true)
                        .Show(true);

                    return;
                }

                onComplete?.Invoke(true, giphyResponse);
            }, profile.token, false, 60);
        });
    }
}
