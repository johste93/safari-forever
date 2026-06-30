using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebError
{
    public long responseCode;
    public string error;
    public string downloadHandlerText;
    public string requestUrl;

    public WebError()
    {}

    public WebError(UnityWebRequest request)
    {
        this.responseCode = request.responseCode;
        this.error = request.error;
        this.requestUrl = request.url;
        this.downloadHandlerText = request.downloadHandler.text;

        if(Globals.debugConstants.verboseLogging)
        {
            Debug.LogError($"{responseCode}: {error}: {downloadHandlerText}");
        }
    }
}
