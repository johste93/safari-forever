using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SlotMachineAPI
{
    private static string betsEndPoint = "slotmachine/bets";
    private static string spinnEndPoint = "slotmachine/spinn";

    public static void GetBets(System.Action<bool, BetsResponse> onComplete)
    {
        bool failSilently = false;

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
            worker.Get(Globals.webConstants.GetHost() + betsEndPoint, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
					onComplete?.Invoke(false, null);
                    return;
                }

                BetsResponse result = JsonConvert.DeserializeObject<BetsResponse>(responseMsg);
                
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

    public static void Spinn(int coinsSpent, System.Action<bool, SpinnResponse> onComplete)
    {
        bool failSilently = false;

        WebRequestMiddleware.FetchAndVerifyProfile(true, (verified, profile)=>
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
            body.Add("CoinsBet", coinsSpent);

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(Globals.webConstants.GetHost() + spinnEndPoint, new Dictionary<string, string>(), body, (success, response)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                SpinnResponse result = JsonConvert.DeserializeObject<SpinnResponse>(response);

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
