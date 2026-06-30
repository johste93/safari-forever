#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using GooglePlayGames.BasicApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using FSG.iOSKeychain;

public class AndroidCloudSave : MonoBehaviour
{
    public static bool playerHasPreviouslySignedIn
    {
        get{
            return PlayerPrefs.GetString("GooglePlayGamesEnabled", "false") == "true";
        }
        set{
            PlayerPrefs.SetString("GooglePlayGamesEnabled", value.ToString().ToLower());
            PlayerPrefs.Save();
        }
    }

    public static void HasBackup(System.Action<bool, bool> onComplete)
    {
        LoadRestoreTokenFromCloud((success, restoreToken)=>
        {
            onComplete(success, success && !string.IsNullOrWhiteSpace(restoreToken));
        });
    }

    public static void SaveRestoreTokenToCloud(string RestoreToken, System.Action<bool> OnComplete)
    {
        Authenticate((success)=>
        {
            if(!success)
            {
                OnComplete(false);
                return;
            }
    
            //Open Savegame
            ((PlayGamesPlatform) Social.Active).SavedGame.OpenWithAutomaticConflictResolution(SaveManager.RestoreTokenKey, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood, (openStatus, openedGame)=>
            {
                if (openStatus != SavedGameRequestStatus.Success)
                {
                    Debug.LogError("Error opening google play savegame: " + openStatus);
                    OnComplete(false);
                    return;
                }

                byte[] bytes = Encoding.UTF8.GetBytes(RestoreToken);

                SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
                SavedGameMetadataUpdate updatedMetadata = builder.Build();
                ((PlayGamesPlatform) Social.Active).SavedGame.CommitUpdate(openedGame, updatedMetadata, bytes, (commitStatus, comittedGame)=>
                {
                    if (commitStatus != SavedGameRequestStatus.Success)
                    {
                        Debug.LogError("Error saving google play savegame: " + commitStatus);
                        OnComplete(false);
                        return;
                    }

                    Debug.Log("google play savegame " + comittedGame.Description + " written");
                    OnComplete(true);
                });
            });
        });
    }

    public static void LoadRestoreTokenFromCloud(System.Action<bool, string> OnComplete)
    {
        Authenticate((success)=>
        {
            if(!success)
            {
                OnComplete(false, "");
                return;
            }

            if(!Social.Active.localUser.authenticated)
            {
                Debug.LogError("User not logged in.");
                OnComplete(false, "");
                return;   
            }

            //Open Savegame
            ((PlayGamesPlatform) Social.Active).SavedGame.OpenWithAutomaticConflictResolution(SaveManager.RestoreTokenKey, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood, (openStatus, game)=>
            {
                if (openStatus != SavedGameRequestStatus.Success)
                {
                    Debug.LogError("Error opening google play savegame: " + openStatus);
                    OnComplete(false, "");
                    return;
                }
               
                //Read Savegame
                ((PlayGamesPlatform) Social.Active).SavedGame.ReadBinaryData(game, (readStatus, cloudData)=>
                {
                    if (readStatus != SavedGameRequestStatus.Success)
                    {
                        Debug.LogError("Error loading google play savegame: " + readStatus);
                        OnComplete(false, "");
                        return;
                    }

                    if (cloudData == null)
                    {
                        Debug.Log("No data saved to the cloud yet...");
                        OnComplete(true, "");
                        return;
                    }

                    string restoreToken = Encoding.UTF8.GetString(cloudData);
                    OnComplete(true, restoreToken);
                });
            });
        });
    }

    public static void Authenticate(System.Action<bool> OnComplete)
    {
        if(!Social.Active.localUser.authenticated)
        {
            // Enable/disable logs on the PlayGamesPlatform
            PlayGamesPlatform.DebugLogEnabled = Application.isEditor || Debug.isDebugBuild;

            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                .EnableSavedGames()
                .Build();

            PlayGamesPlatform.InitializeInstance(config);

            // Activate the Play Games platform. This will make it the default
            // implementation of Social.Active
            PlayGamesPlatform.Activate();

            // Sign in to Google Play Games
            Social.localUser.Authenticate((bool success) =>
            {
                if (!success)
                {
                    // no need to show error message (error messages are shown automatically
                    // by plugin)
                    Debug.LogError("Failed to sign in with Google Play Games.");
                    OnComplete?.Invoke(false);
                    return;
                }

                // if we signed in successfully, load data from cloud
                Debug.Log("Login successful!");
                AndroidCloudSave.playerHasPreviouslySignedIn = true;
                OnComplete?.Invoke(true);
            });
        }
        else
        {
            OnComplete?.Invoke(true);
        }
    }

    public static void DeleteRestoreKey(System.Action<bool> OnComplete)
    {
        if(!Social.Active.localUser.authenticated)
        {
            OnComplete(false);
            return;   
        }

        Social.localUser.Authenticate((bool success) =>
        {
            if (!success)
            {
                OnComplete(false);
                return;
            }

            ((PlayGamesPlatform) Social.Active).SavedGame.OpenWithAutomaticConflictResolution(SaveManager.RestoreTokenKey, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLastKnownGood, (openStatus, game)=>
            {
                if (openStatus != SavedGameRequestStatus.Success)
                {
                    Debug.LogError("Error opening game: " + openStatus);
                    OnComplete(false);
                    return;
                }

                ((PlayGamesPlatform) Social.Active).SavedGame.Delete(game);
                OnComplete(true);
            });
        });
    }
}
#endif