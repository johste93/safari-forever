using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemAPI
{
    public static void GetClientVersion(bool failSilently, System.Action<bool, ClientVersion> onComplete)
	{
        string url = Globals.webConstants.GetHost() + "client/version";

        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.assumeConnectedToInternet = true; //Assume we are already connected to internet
        worker.skipVersionVerification = true; //Skip the built in version verification of the webworker to avoid infinite loop

        worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
        {
            if(!success)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            if(!ClientVersion.Parse(responseMsg, out ClientVersion earliestSupportedVersion))
            {
                Debug.LogError("Unable to parse client version from server");
				onComplete?.Invoke(false, null);
                return;
            }

            onComplete?.Invoke(true, earliestSupportedVersion);

        }, null, failSilently);
	}
}
