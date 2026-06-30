using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FSG.iOSKeychain;

public class Client
{
    public static string clientId
	{
#if UNITY_IOS
		get{
			string key = "ClientId";
			if(string.IsNullOrWhiteSpace(Keychain.GetValue(key)))
				Keychain.SetValue(key, SystemInfo.deviceUniqueIdentifier);

            return Keychain.GetValue(key);
        }
#else
		get{
			return SystemInfo.deviceUniqueIdentifier;
		}
#endif
    }

	public static string GetClientVerificationSecret()
	{	
		string input = Client.clientId + ":" + Globals.clientSecret.salt + ":" + DateTimeOffset.Now.ToUniversalTime().Year + ":" + DateTimeOffset.Now.ToUniversalTime().Month + ":" + DateTimeOffset.Now.ToUniversalTime().Day + ":" + DateTimeOffset.Now.ToUniversalTime().Hour + ":" + DateTimeOffset.Now.ToUniversalTime().Minute;
		string encrypted = RSAEncrypt.Encrypt(input, false).ToBase64();
		return encrypted;
	}

	public static void VerifyVersion(bool failSilently, System.Action<bool, bool> onComplete)
    {
        SystemAPI.GetClientVersion(failSilently, (success, earliestSupportedVersion)=>
        {
            if(!success)
            {
                onComplete?.Invoke(false, false);
                return;
            }

            if(!ClientVersion.Parse(Application.version, out ClientVersion thisVersion))
            {
                Debug.LogError("'Application Version' bad SYNTAX");
                onComplete?.Invoke(false, false);
                return;
            }

            if(!thisVersion.IsNewerThanOrEqual(earliestSupportedVersion))
            {
                Debug.LogError("Client Outdated!");

                if(failSilently)
                {
                    onComplete?.Invoke(true, false);
                    return;
                }

                Language messageLanguage = Localization.KeyAvailable(TranslationKey.Error_OutdatedClient_Body, SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;
                bool messageIsRTL = Localization.IsRightToLeftLanguage(messageLanguage);
			    string message = Localization.GetTranslationFormat2(TranslationKey.Error_OutdatedClient_Body, SaveManager.currentSave.language, thisVersion.ToString(), earliestSupportedVersion.ToString());
                
                new Dialog(
                    TranslationKey.Error_OutdatedClient_Title,
                    message, messageLanguage,
                    messageIsRTL)
                    .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                        onComplete?.Invoke(true, false);
                    })
                    .Show();

                return;
            }

            onComplete?.Invoke(true, true);
        });
    }
}
