using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class TransactionAPI
{
    private static string unrecivedRewardEndPoint = "transaction/unrecived-rewards";
    private static string pricesEndpoint = "transaction/prices";
    private static string unlockCharacterEndpoint = "transaction/unlock-character";
    private static string unlockCharacterWithAppleIAPEndpoint = "transaction/unlock-character-with-apple-iap";
    private static string unlockCharacterWithGoogleIAPEndpoint = "transaction/unlock-character-with-google-iap";

    public static void GetUnrecivedReward(System.Action<bool, List<UnrecivedReward>> onComplete, bool failSilently)
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
            worker.Get(Globals.webConstants.GetHost() + unrecivedRewardEndPoint, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
					onComplete?.Invoke(false, null);
                    return;
                }

                List<UnrecivedReward> result = JsonConvert.DeserializeObject<List<UnrecivedReward>>(responseMsg);

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

    public static void GetPrices(System.Action<bool, Dictionary<string, int>> onComplete, bool failSilently = false)
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
            worker.Get(Globals.webConstants.GetHost() + pricesEndpoint, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
					onComplete?.Invoke(false, null);
                    return;
                }

                Dictionary<string, int> result = JsonConvert.DeserializeObject<Dictionary<string, int>>(responseMsg);

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

    public static void UnlockCharacter(Animal animal, System.Action<bool, UnlockCharacterResponse> onComplete)
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

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
            body.Add("Animal", animal);

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(Globals.webConstants.GetHost() + unlockCharacterEndpoint, new Dictionary<string, string>(), body, (success, response)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                UnlockCharacterResponse result = JsonConvert.DeserializeObject<UnlockCharacterResponse>(response);

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

    public static void UnlockCharacterWithAppleIAP(string base64Receipt, System.Action<bool, UnlockCharacterWithIAPResponse> onComplete)
    {
        bool failSilently = true;

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
            body.Add("base64Receipt", base64Receipt);

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(Globals.webConstants.GetHost() + unlockCharacterWithAppleIAPEndpoint, new Dictionary<string, string>(), body, (success, response)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                UnlockCharacterWithIAPResponse result = JsonConvert.DeserializeObject<UnlockCharacterWithIAPResponse>(response);

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

    public static void UnlockCharacterWithGoogleIAP(string productId, string purchaseToken, System.Action<bool, UnlockCharacterWithIAPResponse> onComplete)
    {
        bool failSilently = true;

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
            body.Add("productId", productId);
            body.Add("purchaseToken", purchaseToken);

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Post(Globals.webConstants.GetHost() + unlockCharacterWithGoogleIAPEndpoint, new Dictionary<string, string>(), body, (success, response)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                UnlockCharacterWithIAPResponse result = JsonConvert.DeserializeObject<UnlockCharacterWithIAPResponse>(response);

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
