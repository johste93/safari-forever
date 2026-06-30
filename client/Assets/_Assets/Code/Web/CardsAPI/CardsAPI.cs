using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class CardsAPI
{
    private static string cardsSelectionEndPoint = "cards/selection";
    private static string pickCardEndPoint = "cards/pick";

    public static void GetCardsSelection(System.Action<bool, CardsSelectionResponse> onComplete)
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
            worker.Get(Globals.webConstants.GetHost() + cardsSelectionEndPoint, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
					onComplete?.Invoke(false, null);
                    return;
                }

                ///Todo Replace result class.
                CardsSelectionResponse result = JsonConvert.DeserializeObject<CardsSelectionResponse>(responseMsg);
                
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

    public static void PickCard(int cardPickedIndex, System.Action<bool, PickCardResponse> onComplete)
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
            body.Add("CardPickedIndex", cardPickedIndex);

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(Globals.webConstants.GetHost() + pickCardEndPoint, new Dictionary<string, string>(), body, (success, response)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                PickCardResponse result = JsonConvert.DeserializeObject<PickCardResponse>(response);

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
