using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;
using FSG.iOSKeychain;

public class Save
{
    public int currentCharacter;
    public bool[] unlockedCharacter = new bool[]{true, false, false, false, false, false, false, false, false, false, false, false, false, false, false};
	public int currentHat;
	public bool[] unlockedHats = new bool[]{true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false};
	public CampaignLevel[][] campaignProgress;
	public List<string> levelsPlayed {get; set; } = new List<string> ();

    //Playerprefs
    public bool music = true;
    public bool sfx = true;
	public bool useRandomCharacter = false;
	public bool useRandomHat = false;
	//public GifQuality gifQuality = GifQuality.Fast;
    public Language language;

    //Accessability
    public float gameSpeed = 0.9f;
    public bool openDyslexic = false;
    public bool monoChromatic = false;
	public bool lowPowerMode = false;
	public bool noBrightFlashesMode = false;

	public GameVersion saveVersion = new GameVersion("0.0");

    public bool needsToUpdateServer = false;

    [JsonProperty]
    private Profile onlineProfile;

	private int ageEntered = -1;
	private bool hasAcceptedTermsOfServiceThisSession;
	private bool hasAcceptedPrivacyPolicyThisSession;

	public delegate void SaveEvent();
	public static SaveEvent OnMergeComplete;

	public Save()
	{
		campaignProgress = new CampaignLevel[3][];
		for(int i = 0; i < 3; i++)
		{
			campaignProgress[i] = new CampaignLevel[12];
			
			for(int j = 0; j < campaignProgress[i].Length; j++)
				campaignProgress[i][j] = new CampaignLevel();
		}
	}

	public bool HasOnlineProfile()
	{
		return onlineProfile != null;
	}

    public void FetchOnlineProfile(System.Action<Profile> OnComplete, bool failSilently = false)
    {
        if(onlineProfile == null)
		{
			StartRegistration(OnComplete, failSilently);
		}
        else
        {
			OnComplete(onlineProfile);
		}
    }

	private void StartRegistration(System.Action<Profile> OnComplete, bool failSilently = false)
	{
		if(!failSilently)
		{
#if UNITY_ANDROID// && !UNITY_EDITOR
			AndroidAutoRestore( (profile)=>
			{
				OnComplete?.Invoke(profile);
				GameObject.FindObjectOfType<ProfileButton>()?.TryShow();
			}, failSilently);
#else
			if(string.IsNullOrWhiteSpace(SaveManager.RestoreToken))
			{
				AgeVerification(OnComplete);
			}
			else
			{
				DialogCanvas.instance.HideLoading();
				new Dialog(TranslationKey.Cloudsaving_Restore_Title, TranslationKey.Cloudsaving_Restore_Body)
					.AddNegativeButton(TranslationKey.Generic_No,()=>
					{
						DOVirtual.DelayedCall(0.3f, ()=>
						{
							new Dialog(TranslationKey.Cloudsaving_Restore_LastChance_Title, TranslationKey.Cloudsaving_Restore_LastChance_Body)
								.AddDestructiveButton(TranslationKey.Cloudsaving_Restore_LastChance_Negative, ()=>
								{
									Keychain.DeleteValue(SaveManager.RestoreTokenKey);
									AgeVerification(OnComplete);
								})
								.AddPositiveButton(TranslationKey.Cloudsaving_Restore_LastChance_Positive, ()=>
								{
									Restore(SaveManager.RestoreToken, (profile)=>
									{
										OnComplete?.Invoke(profile);
										GameObject.FindObjectOfType<ProfileButton>()?.TryShow();
									});
								})
								.Show();
						});
					})
					.AddPositiveButton(TranslationKey.Generic_Ok,()=>
					{
						Restore(SaveManager.RestoreToken,  (profile)=>
						{
							OnComplete?.Invoke(profile);
							GameObject.FindObjectOfType<ProfileButton>()?.TryShow();
						});
					}, true)
					.Show();
			}
#endif
		}
		else
		{
			OnComplete(null);
		}
	}

	private void AgeVerification(System.Action<Profile> OnComplete)
	{
		if(SaveManager.age.Value < 13)
		{
			DialogCanvas.instance.HideLoading();

			Debug.LogError("User not old enough!");
						
			new Dialog(TranslationKey.Generic_Error, TranslationKey.Age_VerificationFailed)
				.AddNeutralButton(TranslationKey.Generic_Ok, null, true)
				.Show();

			return;
		}
		else
		{
			TermsOfServiceRegistration(OnComplete);
		}
	}

	private void TermsOfServiceRegistration(System.Action<Profile> OnComplete)
	{
		System.Action<bool> termsOfServiceCallback = (accepted)=>
		{
			if(accepted)
			{
				hasAcceptedTermsOfServiceThisSession = accepted;
				DOVirtual.DelayedCall(0.2f, ()=>
				{
					PrivacyPolicyRegistration(OnComplete);
				});
			}
			else
			{
				Debug.LogError("User did not accept terms of service.");
				return;
			}
		};

		if(!hasAcceptedTermsOfServiceThisSession)
			DialogCanvas.instance.ShowTermsOfService(termsOfServiceCallback);
		else
			termsOfServiceCallback.Invoke(true);
	}

	private void PrivacyPolicyRegistration(System.Action<Profile> OnComplete)
	{
		System.Action<bool> privacyPolicyCallback = (accepted)=>
		{
			if(accepted)
			{
				hasAcceptedPrivacyPolicyThisSession = accepted;
				DOVirtual.DelayedCall(0.2f, ()=>
				{
					NickNameRegistration(OnComplete);
				});
			}
			else
			{
				Debug.LogError("User did not accept privacy policy.");
				return;
			}
		};

		if(!hasAcceptedPrivacyPolicyThisSession)
			DialogCanvas.instance.ShowPrivacyPolicy(privacyPolicyCallback);
		else
			privacyPolicyCallback.Invoke(true);
	}

	private void NickNameRegistration(System.Action<Profile> OnComplete)
	{
		DialogCanvas.instance.ShowNicknameWindow("", (confirmed, validatedNickname)=>
		{
			if(!confirmed)
				return;
			
			Register(validatedNickname, OnComplete);
		});
	}

	private void Register(string nickname, System.Action<Profile> OnComplete)
	{
		DialogCanvas.instance.ShowLoading();
		UserAPI.Register(nickname, (newProfile)=>
		{
			onlineProfile = newProfile;
			if(onlineProfile != null)
			{
				SaveManager.Save();
				GameObject.FindObjectOfType<ProfileButton>()?.TryShow();
			}
				
			DialogCanvas.instance.HideLoading();
			OnComplete(newProfile);
		});	
	}

	public void Restore(string restoreToken, System.Action<Profile> OnComplete)
	{
		if(string.IsNullOrEmpty(restoreToken))
		{
			DialogCanvas.instance.HideLoading();
			new Dialog(TranslationKey.Generic_Error, TranslationKey.Cloudsaving_Restore_Error_MissingRestoreKey_Body)
				.AddNeutralButton(TranslationKey.Generic_Ok, null)
				.Show();
				
			return;
		}

		DialogCanvas.instance.ShowLoading();
		UserAPI.Restore(restoreToken, (success, restoredProfile)=>
		{
			if(!success)
			{
				DialogCanvas.instance.HideLoading();
				Debug.LogError("Restore failed.");
				OnComplete(null);
				return;
			}

			SaveManager.currentSave.onlineProfile = restoredProfile;
			SaveManager.Save();

			UserAPI.DownloadBackup((downloadSuccess)=>
			{
				DialogCanvas.instance.HideLoading();
				OnComplete(SaveManager.currentSave.onlineProfile);
			}, false);
		});
	}

	private void AndroidAutoRestore(System.Action<Profile> OnComplete, bool failSilently = false)
	{
#if UNITY_ANDROID
		if(Social.Active.localUser.authenticated)
        {
			DialogCanvas.instance.ShowLoading();
			//Check if player has played before
			AndroidCloudSave.LoadRestoreTokenFromCloud((loadSuccess, restoreToken)=>
			{
				if(!loadSuccess)
				{
					AndroidManualRestore(OnComplete, failSilently);
					return;
				}

				if(string.IsNullOrWhiteSpace(restoreToken))
				{
					Keychain.DeleteValue(SaveManager.RestoreTokenKey);
					AndroidCloudSave.DeleteRestoreKey((success)=>{
						//AndroidCloudSave.playerHasPreviouslySignedIn = false;
					});
					AgeVerification(OnComplete);
					return;
				}

				DialogCanvas.instance.HideLoading();
				new Dialog(TranslationKey.Cloudsaving_Restore_Title, TranslationKey.Cloudsaving_Restore_Body)
					.AddNegativeButton(TranslationKey.Generic_No, ()=>
					{
						DOVirtual.DelayedCall(0.2f, ()=>
						{
							new Dialog(TranslationKey.Cloudsaving_Restore_LastChance_Title, TranslationKey.Cloudsaving_Restore_LastChance_Body)
								.AddDestructiveButton(TranslationKey.Cloudsaving_Restore_LastChance_Negative, ()=>
								{
									Keychain.DeleteValue(SaveManager.RestoreTokenKey);
									AndroidCloudSave.DeleteRestoreKey((success)=>{
										//AndroidCloudSave.playerHasPreviouslySignedIn = false;
									});
									AgeVerification(OnComplete);
								})
								.AddPositiveButton(TranslationKey.Cloudsaving_Restore_LastChance_Positive, ()=>
								{
									Restore(restoreToken, OnComplete);
								})
								.Show();
						});
					})
					.AddPositiveButton(TranslationKey.Generic_Yes, ()=>
					{
						Restore(restoreToken, OnComplete);
					}, true)
					.Show();
			});
		}
		else
		{
			//Ask if player has played before.
			AndroidManualRestore(OnComplete, failSilently);
		}
#endif
	}

	private void AndroidManualRestore(System.Action<Profile> OnComplete, bool failSilently = false)
	{
#if UNITY_ANDROID
		DialogCanvas.instance.HideLoading();
		new Dialog(TranslationKey.Cloudsaving_GooglePlayGames_Preregistration_PlayedBefore_Title, TranslationKey.Cloudsaving_GooglePlayGames_Preregistration_PlayedBefore_Body)
			.AddNegativeButton(TranslationKey.Generic_No, ()=>
			{
				AgeVerification(OnComplete);
			})
			.AddPositiveButton(TranslationKey.Generic_Yes, ()=>
			{
				AndroidAttemptRestore(OnComplete, failSilently);
			}).Show();
#endif
	}

	private void AndroidAttemptRestore(System.Action<Profile> OnComplete, bool failSilently = false)
	{
#if UNITY_ANDROID
		//Sign in with google play.
		DialogCanvas.instance.ShowLoading();
		AndroidCloudSave.Authenticate((loginSuccess)=>
		{
			if(!loginSuccess)
			{
				DialogCanvas.instance.HideLoading();
				//Error not sign in. Ask player what to do:
				// Try again
				// Cancel
				new Dialog(TranslationKey.Cloudsaving_GooglePlayGames_Preregistration_Error_SignIn_Title, TranslationKey.Cloudsaving_GooglePlayGames_Preregistration_Error_SignIn_Body)
				.AddNeutralButton(TranslationKey.Generic_TryAgain, ()=>
				{
					AndroidAttemptRestore(OnComplete, failSilently);
				})
				.AddNegativeButton(TranslationKey.Generic_Cancel, ()=>
				{
					OnComplete(null);
				})
				.Show();

				return;
			}

			AndroidCloudSave.LoadRestoreTokenFromCloud((loadSuccess, restoreToken)=>
			{
				if(!loadSuccess)
				{
					DialogCanvas.instance.HideLoading();
					//Error unable to get token. Ask player what to do:
					// Try again
					// Cancel
					new Dialog(TranslationKey.Cloudsaving_GooglePlayGames_Preregistration_Error_Fetch_Title, TranslationKey.Cloudsaving_GooglePlayGames_Preregistration_Error_Fetch_Body)
						.AddNeutralButton(TranslationKey.Generic_TryAgain, ()=>
						{
							AndroidAttemptRestore(OnComplete, failSilently);
						})
						.AddNegativeButton(TranslationKey.Generic_Cancel, ()=>
						{
							OnComplete(null);
						})
						.Show();
					return;
				}

				
				if(string.IsNullOrWhiteSpace(restoreToken))
				{
					DialogCanvas.instance.HideLoading();
					
					//Error no restore token found.
					// Cancel
					// Create new account
					new Dialog(TranslationKey.Cloudsaving_GooglePlayGames_Preregistration_Error_NoKey_Title, TranslationKey.Cloudsaving_GooglePlayGames_Preregistration_Error_NoKey_Body)
						.AddNegativeButton(TranslationKey.Generic_Cancel, ()=>
						{
							OnComplete(null);
						})
						.AddNegativeButton(TranslationKey.Cloudsaving_GooglePlayGames_Preregistration_Error_NoKey_CreateAccount, ()=>
						{
							DOVirtual.DelayedCall(0.2f, ()=>
							{
								new Dialog(TranslationKey.Cloudsaving_Restore_LastChance_Title, TranslationKey.Cloudsaving_Restore_LastChance_Body)
									.AddDestructiveButton(TranslationKey.Cloudsaving_Restore_LastChance_Negative,()=>
									{
										Keychain.DeleteValue(SaveManager.RestoreTokenKey);
										AndroidCloudSave.DeleteRestoreKey((success)=>{
											//AndroidCloudSave.playerHasPreviouslySignedIn = false;
										});
										
										AgeVerification(OnComplete);
									})
									.AddPositiveButton(TranslationKey.Cloudsaving_Restore_LastChance_Positive, ()=>
									{
										AndroidAttemptRestore(OnComplete, failSilently);
									});
							});
						})
						.Show();
					return;
				}

				Restore(restoreToken, OnComplete);
			});
		});
#endif
	}

    public void Migrate() //This needs to run every time a character is added;
    {
        Save defaultValues = new Save();

        bool[] old = unlockedCharacter;
        unlockedCharacter = defaultValues.unlockedCharacter;
        
        for(int i = 0; i < old.Length; i++)
            unlockedCharacter[i] = old[i];
    }

	public void Merge(SaveDataDTO saveDataDTO)
	{
		onlineProfile.nickname = saveDataDTO.Nickname;
		onlineProfile.identifier = saveDataDTO.Identifier;
		onlineProfile.color = saveDataDTO.Color;
		onlineProfile.coins = saveDataDTO.Coins;
		
		unlockedCharacter[(int)Animal.Pepe] = true;
		unlockedCharacter[(int)Animal.Patchy] = saveDataDTO.PatchyUnlocked;
		unlockedCharacter[(int)Animal.Jaws] = saveDataDTO.JawsUnlocked;
		unlockedCharacter[(int)Animal.Olipher] = saveDataDTO.OlipherUnlocked;
		unlockedCharacter[(int)Animal.Koko] = saveDataDTO.KokoUnlocked;
		unlockedCharacter[(int)Animal.Leon] = saveDataDTO.LeonUnlocked;
		unlockedCharacter[(int)Animal.Debra] = saveDataDTO.DebraUnlocked;
		unlockedCharacter[(int)Animal.Nugget] = saveDataDTO.NuggetUnlocked;
		unlockedCharacter[(int)Animal.Perry] = saveDataDTO.PerryUnlocked;
		unlockedCharacter[(int)Animal.Rex] = saveDataDTO.RexUnlocked;
		unlockedCharacter[(int)Animal.Pingo] = saveDataDTO.PingoUnlocked;
		unlockedCharacter[(int)Animal.Honky] = saveDataDTO.HonkyUnlocked;
		unlockedCharacter[(int)Animal.Speedy] = saveDataDTO.SpeedyUnlocked;
		unlockedCharacter[(int)Animal.Brine] = saveDataDTO.BrineUnlocked;
		unlockedCharacter[(int)Animal.Axol] = saveDataDTO.AxolUnlocked;

		unlockedHats[(int)Hat.Santa] = saveDataDTO.SantaHatUnlocked;
		unlockedHats[(int)Hat.Shades] = saveDataDTO.ShadesHatUnlocked;
		unlockedHats[(int)Hat.Thinfoil] = saveDataDTO.ThinfoilHatUnlocked;
		unlockedHats[(int)Hat.Wizzard] = saveDataDTO.WizzardHatUnlocked;
		unlockedHats[(int)Hat.Witch] = saveDataDTO.WitchHatUnlocked;
		unlockedHats[(int)Hat.Pirate] = saveDataDTO.PirateHatUnlocked;
		unlockedHats[(int)Hat.Showbiz] = saveDataDTO.ShowbizHatUnlocked;
		unlockedHats[(int)Hat.Halo] = saveDataDTO.HaloHatUnlocked;
		unlockedHats[(int)Hat.TopHat] = saveDataDTO.TopHatHatUnlocked;
		unlockedHats[(int)Hat.Viking] = saveDataDTO.VikingHatUnlocked;
		unlockedHats[(int)Hat.Horns] = saveDataDTO.HornsHatUnlocked;
		unlockedHats[(int)Hat.Sombrero] = saveDataDTO.SombreroHatUnlocked;
		unlockedHats[(int)Hat.Conical] = saveDataDTO.ConicalHatUnlocked;
		unlockedHats[(int)Hat.Boot] = saveDataDTO.BootHatUnlocked;
		unlockedHats[(int)Hat.Comrade] = saveDataDTO.ComradeHatUnlocked;
		unlockedHats[(int)Hat.Crown] = saveDataDTO.CrownHatUnlocked;
		unlockedHats[(int)Hat.Mustache] = saveDataDTO.MustacheHatUnlocked;
		unlockedHats[(int)Hat.Beanie] = saveDataDTO.BeanieHatUnlocked;
		unlockedHats[(int)Hat.SouWester] = saveDataDTO.SouWesterHatUnlocked;
		unlockedHats[(int)Hat.Private] = saveDataDTO.PrivateHatUnlocked;

		levelsPlayed = saveDataDTO.LevelsPlayed;


		foreach(UserCampaignLevelDataDTO levelData in saveDataDTO.UserCampaignLevelData)
		{
			campaignProgress[levelData.World][levelData.Index].beaten = campaignProgress[levelData.World][levelData.Index].beaten || levelData.Beaten;

			if( (levelData.Seconds > 0 || levelData.Milliseconds > 0) && levelData.Beaten)
			{
				if(campaignProgress[levelData.World][levelData.Index].seconds < 0 || campaignProgress[levelData.World][levelData.Index].milliseconds < 0)
				{
					campaignProgress[levelData.World][levelData.Index].seconds = levelData.Seconds;
					campaignProgress[levelData.World][levelData.Index].milliseconds = levelData.Milliseconds;
				}
				else
				{
					Highscore localHighscore = new Highscore(campaignProgress[levelData.World][levelData.Index].seconds, campaignProgress[levelData.World][levelData.Index].milliseconds);
					Highscore serverHighscore = new Highscore(levelData.Seconds, levelData.Milliseconds);
					if(localHighscore.IsLowerThanOrEqual(serverHighscore))
					{
						campaignProgress[levelData.World][levelData.Index].seconds = localHighscore.Seconds;
						campaignProgress[levelData.World][levelData.Index].milliseconds = localHighscore.Milliseconds;
					}
					else
					{
						campaignProgress[levelData.World][levelData.Index].seconds = serverHighscore.Seconds;
						campaignProgress[levelData.World][levelData.Index].milliseconds = serverHighscore.Milliseconds;
					}
				}
			}		
		}

		OnMergeComplete?.Invoke();
	}

	
    public bool IsLocalUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return false;

		if(onlineProfile == null)
			return false;
        
        return onlineProfile.userId.Equals(userId);
    }
}
