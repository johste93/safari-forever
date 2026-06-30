using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FSG.iOSKeychain;

public class UserAPI
{
    private const string enableRestoreEndpoint = "user/enable-restore";
    private const string registerEndpoint = "user/register";
    private const string restoreEndpoint = "user/restore";
    
    private const string uploadBackupEndoint = "user/upload-backup";
    private const string downloadBackupEndpoint = "user/download-backup";

    public static void FetchProfile(string userId, System.Action<bool, ProfileResponse> onComplete, bool failSilently = false)
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
                
            //Pull profile.
            string url = Globals.webConstants.GetHost() + $"user/{userId}/profile";

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                ProfileResponse result = JsonConvert.DeserializeObject<ProfileResponse>(responseMsg);

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
            }, profile.token);
        });
    }

    public static void FetchProfile(string nickname, string identifier, System.Action<bool, ProfileResponse> onComplete, bool failSilently = false)
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
                
            //Pull profile.
            string url = Globals.webConstants.GetHost() + $"user/{nickname}/{identifier}/profile";

            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                ProfileResponse result = JsonConvert.DeserializeObject<ProfileResponse>(responseMsg);

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
            }, profile.token);
        });
    }

    public static void Register(string nickname, System.Action<Profile> onComplete)
    {
        SortedDictionary<string, object> body = new SortedDictionary<string, object>();
        body.Add("Nickname", nickname);

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("ClientId", Client.clientId);
        headers.Add("ClientSecret", Client.GetClientVerificationSecret());

        string url = Globals.webConstants.GetHost() + registerEndpoint;
        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.Post(url, headers, body, (success, result) =>
        {
            if(!success)
            {
                onComplete?.Invoke(null);
                return; 
            }
            
            NewUserResponse newUserResponse = JsonConvert.DeserializeObject<NewUserResponse>(result);
            if(newUserResponse == null)
            {
                new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                    .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                        onComplete(null);
                    }, true)
                    .Show(true);

                return;
            }

            if (Globals.webConstants.useLocalHost)
            {
                Debug.Log(newUserResponse.token);
                Debug.Log(newUserResponse.restoreToken);
            }

            Keychain.SetValue(SaveManager.RestoreTokenKey, newUserResponse.restoreToken);

#if UNITY_ANDROID// && !UNITY_EDITOR

            if(!Social.Active.localUser.authenticated)
            {
                DialogCanvas.instance.HideLoading();
                new Dialog(TranslationKey.Cloudsaving_Restore_Enable_Title, TranslationKey.Cloudsaving_Restore_Enable_Body)
                    .AddNegativeButton(TranslationKey.Generic_Cancel, ()=>{
                        onComplete(new Profile(newUserResponse.userId, newUserResponse.token, newUserResponse.saveData.Nickname, newUserResponse.saveData.Identifier, newUserResponse.saveData.Color, newUserResponse.saveData.Coins));
                    })
                    .AddPositiveButton(TranslationKey.Generic_Confirm,()=>
                    {
                        AndroidUploadRestoreToken(false, ()=>{
                            onComplete(new Profile(newUserResponse.userId, newUserResponse.token, newUserResponse.saveData.Nickname, newUserResponse.saveData.Identifier, newUserResponse.saveData.Color, newUserResponse.saveData.Coins));
                        });
                    }, true).Show();
            }
            else
            {
                AndroidUploadRestoreToken(true, ()=>{
                    onComplete(new Profile(newUserResponse.userId, newUserResponse.token, newUserResponse.saveData.Nickname, newUserResponse.saveData.Identifier, newUserResponse.saveData.Color, newUserResponse.saveData.Coins));
                });
            }           
#else   
            onComplete(new Profile(newUserResponse.userId, newUserResponse.token, newUserResponse.saveData.Nickname, newUserResponse.saveData.Identifier, newUserResponse.saveData.Color, newUserResponse.saveData.Coins));
#endif            
        });
    }

    private static void AndroidUploadRestoreToken(bool silent, System.Action OnComplete)
    {
#if UNITY_ANDROID// && !UNITY_EDITOR
        DialogCanvas.instance.ShowLoading();
        AndroidCloudSave.Authenticate((loginSuccess)=>
        {
            if(!loginSuccess)
            {
                DialogCanvas.instance.HideLoading();
                new Dialog(TranslationKey.Cloudsaving_GooglePlayGames_Postregistration_Error_SignIn_Title, TranslationKey.Cloudsaving_GooglePlayGames_Postregistration_Error_SignIn_Body)
                    .AddNeutralButton(TranslationKey.Generic_Ok, null)
                    .Show();

                OnComplete();
                return;
            }

            //Upload Key
            AndroidCloudSave.SaveRestoreTokenToCloud(SaveManager.RestoreToken, (saveSuccess)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!saveSuccess)
                {   
                    new Dialog(TranslationKey.Cloudsaving_GooglePlayGames_Postregistration_Error_FailedUpload_Title, TranslationKey.Cloudsaving_GooglePlayGames_Postregistration_Error_FailedUpload_Body)
                        .AddNeutralButton(TranslationKey.Generic_Ok, null)
                        .Show();

                    OnComplete();
                    return;
                }

                if(!silent)
                {
                    new Dialog(TranslationKey.Cloudsaving_Restore_Enabled_Title, TranslationKey.Cloudsaving_Restore_Enabled_Body)
                        .AddPositiveButton(TranslationKey.Generic_Ok,null)
                        .Show();
                }

                OnComplete();
            });
        });
#endif
    }

    public static void Restore(string restoreToken, System.Action<bool, Profile> onComplete)
    {
        SortedDictionary<string, object> body = new SortedDictionary<string, object>();
        body.Add("RestoreToken", restoreToken);
        body.Add("ClientId", Client.clientId);

        string url = Globals.webConstants.GetHost() + restoreEndpoint;
        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.Post(url, new Dictionary<string, string>(), body, (success, result) =>
        {
            if(!success)
            {
                onComplete?.Invoke(false, null);
                return; 
            }

            ProfileRestoreResponse restoreResponse = JsonConvert.DeserializeObject<ProfileRestoreResponse>(result);

            if(restoreResponse == null)
            {
                new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                    .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                    .Show(true);

                onComplete?.Invoke(false, null);
                return;
            }

            if (Globals.webConstants.useLocalHost)
                Debug.Log(restoreResponse.Token);

            onComplete?.Invoke(true, new Profile(restoreResponse.UserId, restoreResponse.Token, restoreResponse.Nickname, restoreResponse.Identifier, restoreResponse.Color, restoreResponse.Coins));
        });
    }

    public static void UploadBackup(System.Action<bool> onComplete)
	{
		if(!SaveManager.currentSave.needsToUpdateServer)
		{
            onComplete?.Invoke(false);
            return;
        }

        WebRequestMiddleware.FetchAndVerifyProfile(true, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false);
                return;
            }

            if(profile == null)
            {
                onComplete?.Invoke(false);
                return;
            }

            List<UserCampaignLevelDataDTO> UserCampaignLevelData = new List<UserCampaignLevelDataDTO>();
            int worldIndex = 0;
            foreach(CampaignLevel[] array in SaveManager.currentSave.campaignProgress )
            {
                int levelIndex = 0;
                foreach(CampaignLevel level in array )
                {
                    UserCampaignLevelData.Add(new UserCampaignLevelDataDTO(){
                        World = worldIndex,
                        Index = levelIndex,
                        Beaten = level.beaten,
                        Seconds = level.seconds,
                        Milliseconds = level.milliseconds
                    });
                    levelIndex++;
                }

                worldIndex++;
            }

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
            body.Add("UserCampaignLevelData", UserCampaignLevelData);

            /*
            body.Add("PepeUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Pepe]);
            body.Add("PatchyUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Patchy]);
            body.Add("JawsUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Jaws]);
            body.Add("OlipherUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Olipher]);
            body.Add("KokoUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Koko]);
            body.Add("LeonUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Leon]);
            body.Add("DebraUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Debra]);
            body.Add("NuggetUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Nugget]);
            body.Add("PerryUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Perry]);
            body.Add("RexUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Rex]);
            body.Add("PingoUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Pingo]);
            body.Add("HonkyUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Honky]);
            body.Add("SpeedyUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Speedy]);
            body.Add("BrineUnlocked", SaveManager.currentSave.unlockedCharacter[(int)Animal.Brine]);
            */
            
            string url = Globals.webConstants.GetHost() + uploadBackupEndoint;
			WebWorker worker = WebWorkerFactory.HireWorker();
			worker.Post(url, new Dictionary<string, string>(), body, (success, result)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                SaveManager.currentSave.needsToUpdateServer = false;
                SaveManager.Save();
                onComplete?.Invoke(true);
            }, profile.token);

		});
	}

    public static void DownloadBackup(System.Action<bool> onComplete, bool failSilently)
	{
        //We dont require verification to download backup.
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
            {
                onComplete?.Invoke(false);
                return;
            }

            string url = Globals.webConstants.GetHost() + downloadBackupEndpoint;

			WebWorker worker = WebWorkerFactory.HireWorker();
			worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
            {
                if(!success)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                //Parse
                SaveDataDTO saveDataDTO = JsonConvert.DeserializeObject<SaveDataDTO>(responseMsg);

                if(saveDataDTO == null)
                {
                    if(failSilently)
                    {
                        onComplete?.Invoke(false);
                        return;
                    }

                    new Dialog(TranslationKey.Generic_Error, "Unable to deserialize SaveDataDTO", Globals.localizationConstants.defaultLanguage, false)
                        .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                            onComplete?.Invoke(false);
                        }, true)
                        .Show(true);
                    return;
                }

                SaveManager.currentSave.Merge(saveDataDTO);

                //Debug.Log("Backup downloaded successfully!");

                onComplete?.Invoke(true);
            }, profile.token, failSilently);
		}, true);
	}

    public static void EnableRestore(System.Action<bool> onComplete)
    {
        WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
        {
            if(!verified)
            {
                onComplete?.Invoke(false);
                return;
            }

            if(profile == null)
            {
                onComplete?.Invoke(false);
                return;
            }

            DialogCanvas.instance.ShowLoading();

            string url = Globals.webConstants.GetHost() + enableRestoreEndpoint;
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data) =>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                {
                    onComplete?.Invoke(false);
                    return;
                }

                Keychain.SetValue(SaveManager.RestoreTokenKey, responseMsg);
                onComplete?.Invoke(true);

            }, profile.token);
        });
    }

    public static void FetchTermsOfServiceStatus(Profile profile, bool failSilently, System.Action<bool, bool> onComplete)
    {
        string url = Globals.webConstants.GetHost() + "user/get-terms-of-service-agreement-status";
        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
        {
            if(!success)
            {
                onComplete?.Invoke(false, false);
                return;
            }

            onComplete?.Invoke(true, responseMsg == "true");
        }, profile.token, failSilently);
    }

    public static void UpdateTermsOfServiceStatus(Profile profile, bool accept, System.Action<bool> onComplete)
    {
        SortedDictionary<string, object> body = new SortedDictionary<string, object>();
        body.Add("Accept", accept);

        string url = Globals.webConstants.GetHost() + "user/update-terms-of-service-agreement";
        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.Patch(url, new Dictionary<string, string>(), body, (success, responseMsg)=>
        {
            if(!success)
            {
                onComplete?.Invoke(false);
                return;
            }

            onComplete?.Invoke(true);
        }, profile.token);
    }

    public static void FetchPrivacyPolicyStatus(Profile profile, bool failSilently, System.Action<bool, bool> onComplete)
    {
        WebWorker worker = WebWorkerFactory.HireWorker();
        string url = Globals.webConstants.GetHost() + "user/get-privacy-policy-agreement-status";
        worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
        {
            if(!success)
            {
                onComplete(false, false);
                return;
            }

            onComplete(true, responseMsg == "true");
        }, profile.token, failSilently);
    }

    public static void UpdatePrivacyPolicyStatus(Profile profile, bool accept, System.Action<bool> onComplete)
    {
        SortedDictionary<string, object> body = new SortedDictionary<string, object>();
        body.Add("Accept", accept);

        string url = Globals.webConstants.GetHost() + "user/update-privacy-policy-agreement";
        WebWorker worker = WebWorkerFactory.HireWorker();
        worker.Patch(url, new Dictionary<string, string>(), body, (success, responseMsg)=>
        {
            if(!success)
            {
                onComplete?.Invoke(false);
                return;
            }

            onComplete?.Invoke(true);
        }, profile.token);
    }

    public static void RequestAllUserData(System.Action<bool, string> onComplete)
	{
		SaveManager.currentSave.FetchOnlineProfile((profile)=>
		{
			if(profile == null)
			{
                onComplete?.Invoke(false, null);
                return;
            }

			DialogCanvas.instance.ShowLoading();

			WebWorker worker = WebWorkerFactory.HireWorker();
			string url = Globals.webConstants.GetHost() + "user/rightToAccessRequest";
			worker.Get(url, new Dictionary<string, string>(), (success, responseMsg, data)=>
			{
				DialogCanvas.instance.HideLoading();
				if(!success)
				{
					onComplete?.Invoke(false, null);
					return;
				}	

				JObject jObject = JObject.Parse(responseMsg);
				responseMsg = jObject.ToString();

                onComplete?.Invoke(true, responseMsg);
			
			}, profile.token);
		});
	}

    public static void UpdateNickname(string newNickname, System.Action<bool, ChangeNicknameResponse> onComplete)
    {
        WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
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

            DialogCanvas.instance.ShowLoading();

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
            body.Add("Nickname", newNickname);

            string url = Globals.webConstants.GetHost() + "user/update-nickname";
            WebWorker worker = WebWorkerFactory.HireWorker();
            worker.Patch(url, new Dictionary<string, string>(), body, (success, responseMsg)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                ChangeNicknameResponse result = JsonConvert.DeserializeObject<ChangeNicknameResponse>(responseMsg);
               
                if(result == null)
                {
                    new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                            onComplete?.Invoke(false, null);
                        }, true)
                        .Show(true);
                    return;
                }

                onComplete?.Invoke(true, result);
            }, profile.token);
        });
    }

    public static void UpdateColor(string hexColor, System.Action<bool, ChangeColorResponse> onComplete)
    {
        WebRequestMiddleware.FetchAndVerifyProfile(false, (verified, profile)=>
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

            DialogCanvas.instance.ShowLoading();

            SortedDictionary<string, object> body = new SortedDictionary<string, object>();
            body.Add("Color", hexColor);

            string url = Globals.webConstants.GetHost() + "user/update-color";
            WebWorker worker = WebWorkerFactory.HireWorker();
            
            worker.Patch(url, new Dictionary<string, string>(), body, (success, responseMsg)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                {
                    onComplete?.Invoke(false, null);
                    return;
                }

                ChangeColorResponse result = JsonConvert.DeserializeObject<ChangeColorResponse>(responseMsg);

                if(result == null)
                {
                    new Dialog(TranslationKey.Generic_ServerError, TranslationKey.Generic_InvalidResponse)
                        .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                            onComplete?.Invoke(false, null);
                        }, true)
                        .Show(true);
                    return;
                }

                onComplete?.Invoke(true, result);
            }, profile.token);
        });
    }
}
