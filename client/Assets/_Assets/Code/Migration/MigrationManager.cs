using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSG.iOSKeychain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using Chumpware.Security;

public class MigrationManager
{
    public static void Migrate(System.Action OnComplete)
    {
        //Try loading without encryption.
        if(!SaveManager.IsEncryped())
            SaveManager.Load(false);

        UpdateTo_0_79(()=>{
            UpdateTo_0_83(()=>{
                UpdateTo_0_84(()=>{
                    UpdateTo_0_89(()=>{
                        UpdateTo_0_97(()=>{
                            UpdateTo_1_3(()=>{
                                UpdateTo_1_4(()=>{
                                    OnComplete();
                                });
                            });
                        });
                    });
                });
            });
        });
    }

    private static void UpdateTo_0_79(System.Action OnComplete)
    {
        string patchVersion = "0.79";
        if(SaveManager.currentSave.saveVersion.IsNewerThanOrEqual(new GameVersion(patchVersion)))
        {
            OnComplete();
            return;
        }

        Debug.Log($"Migrating to: v{patchVersion}");

        SaveManager.currentSave.needsToUpdateServer = true;

        SaveManager.currentSave.saveVersion = new GameVersion(patchVersion);

        SaveManager.Save();

		Debug.Log($"Migration to {patchVersion} Complete!");
        OnComplete();
    }

    private static void UpdateTo_0_83(System.Action OnComplete)
    {
        string patchVersion = "0.83";
        if(SaveManager.currentSave.saveVersion.IsNewerThanOrEqual(new GameVersion(patchVersion)))
        {
            OnComplete();
            return;
        }

        Debug.Log($"Migrating to: v{patchVersion}");

        //This does not handle user not having a profile!
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
            {
                //No migration is necceary
                SaveManager.currentSave.saveVersion = new GameVersion(patchVersion);
                SaveManager.Save();
                OnComplete();
                return;
            }

            UserAPI.FetchProfile(profile.userId, (success, profileResponse)=>
            {
                if(!success)
                    return;

                profile.nickname = profileResponse.Nickname;
                profile.identifier = profileResponse.Identifier;

                SaveManager.currentSave.saveVersion = new GameVersion(patchVersion);
                SaveManager.Save(false);

				Debug.Log($"Migration to {patchVersion} Complete!");
                OnComplete();

            }, false);
        }, true);
    }

    private static void UpdateTo_0_84(System.Action OnComplete)
    {
        string patchVersion = "0.84";
        if(SaveManager.currentSave.saveVersion.IsNewerThanOrEqual(new GameVersion(patchVersion)))
        {
            OnComplete();
            return;
        }

        Debug.Log($"Migrating to: v{patchVersion}");

        //This does not handle user not having a profile!
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
            {
                //No migration is necceary
                SaveManager.currentSave.saveVersion = new GameVersion(patchVersion);
                SaveManager.Save();
                OnComplete();
                return;
            }

            UserAPI.FetchProfile(profile.userId, (success, profileResponse)=>
            {
                if(!success)
                    return;

                profile.color = profileResponse.Color;

                SaveManager.currentSave.saveVersion = new GameVersion(patchVersion);
                SaveManager.Save();

                Debug.Log($"Migration to {patchVersion} Complete!");
                OnComplete();
            }, false);
        }, true);
    }

    private static void UpdateTo_0_89(System.Action OnComplete)
    {
        string patchVersion = "0.89";
        if(SaveManager.currentSave.saveVersion.IsNewerThanOrEqual(new GameVersion(patchVersion)))
        {
            OnComplete();
            return;
        }

        //SaveManager.DeleteWorkInProgress();

        SaveManager.currentSave.saveVersion = new GameVersion(patchVersion);
        SaveManager.Save(false);

        OnComplete();
    }

    private static void UpdateTo_0_97(System.Action OnComplete)
    {
        string patchVersion = "0.97";
        if(SaveManager.currentSave.saveVersion.IsNewerThanOrEqual(new GameVersion(patchVersion)))
        {
            OnComplete();
            return;
        }

        if(SaveManager.currentSave.gameSpeed > 0.9f)
            SaveManager.currentSave.gameSpeed = 0.9f;

        SaveManager.currentSave.saveVersion = new GameVersion(patchVersion);
        SaveManager.Save(false);
        OnComplete();
    }

    private static void UpdateTo_1_3(System.Action OnComplete)
    {
        string patchVersion = "1.3";
        if(SaveManager.currentSave.saveVersion.IsNewerThanOrEqual(new GameVersion(patchVersion)))
        {
            OnComplete();
            return;
        }
        
        string pathToOldWip = Path.Combine(Application.persistentDataPath, "wip.json");

        if(File.Exists(pathToOldWip))
        {
            string newName = $"{System.DateTime.Now.ToString("HH:mm - dd / MMM / yyyy")}";

            string wip = File.ReadAllText(pathToOldWip);
            string cyphered = StringCipher.Encrypt(wip, "s85%Z5JdfK6OP^IH");
            

            Directory.CreateDirectory(Garage.directoryPath);
            File.WriteAllText(Garage.GetWorkInProgressLevelPath(newName), cyphered);

            SaveManager.currentWorkInProgressName = newName;

            File.Delete(pathToOldWip);
        }

        SaveManager.currentSave.saveVersion = new GameVersion(patchVersion);
        SaveManager.Save();
        OnComplete();
    }

    private static void UpdateTo_1_4(System.Action OnComplete)
    {
        string patchVersion = "1.4";
        if(SaveManager.currentSave.saveVersion.IsNewerThanOrEqual(new GameVersion(patchVersion)))
        {
            OnComplete();
            return;
        }
        
        SaveManager.currentSave.lowPowerMode = false;
        SaveManager.currentSave.gameSpeed = 0.9f;

        for(int world = 0; world < SaveManager.currentSave.campaignProgress.Length; world++)
        {
            for(int level = 0; level < SaveManager.currentSave.campaignProgress[world].Length; level++)
            {
                double d = SaveManager.currentSave.campaignProgress[world][level].localHighscore;
                SaveManager.currentSave.campaignProgress[world][level].seconds = (int)d;
                SaveManager.currentSave.campaignProgress[world][level].milliseconds = (int)((d - ((int)d)) * 100) + 1;
            }
        }

        
        SaveManager.currentSave.saveVersion = new GameVersion(patchVersion);
        SaveManager.Save();
        OnComplete();
    }
}
