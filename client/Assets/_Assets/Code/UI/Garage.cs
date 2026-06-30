using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Globalization;
using DG.Tweening;
using Chumpware.Security;

public class Garage
{
    private const string passPhrase = "s85%Z5JdfK6OP^IH";

    public static string directoryPath{
        get{
            return Path.Combine(GetSavePath(), "WIP");
        }
    }

    public static string GetWorkInProgressLevelPath(string levelName)
    {
        string fileName = WWW.EscapeURL(levelName);
        return Path.Combine(Garage.directoryPath, $"{fileName}.wip");
    }

    public static void DeleteWorkInProgressLevel()
    {
        DeleteWorkInProgressLevel(SaveManager.currentWorkInProgressName);
    }

    public static void DeleteWorkInProgressLevel(string levelName)
    {
        if(File.Exists(GetWorkInProgressLevelPath(levelName)))
            File.Delete(GetWorkInProgressLevelPath(levelName));
    }

    public static void SaveWorkInProgressLevel()
    {
        if(string.IsNullOrWhiteSpace(SaveManager.currentWorkInProgressName))
        {
            //string[] name = NameGenerator.RerollAll();
            SaveManager.currentWorkInProgressName = $"{DateTime.Now.ToString("HH:mm - dd / MMM / yyyy")}";
        }    

        SaveWorkInProgressLevel(SaveManager.currentWorkInProgressName);
    }

    public static void SaveWorkInProgressLevel(string levelName)
    {
        ThumbnailCamera.instance.Snap();
        string wip = LevelSerializer.Save(true);
        string ciphered = StringCipher.Encrypt(wip, passPhrase);

        Directory.CreateDirectory(directoryPath);

        System.IO.File.WriteAllText(GetWorkInProgressLevelPath(levelName), ciphered);
    }

    public static bool HasWorkInProgress()
    {
        return Directory.Exists(directoryPath) && Directory.GetFiles(directoryPath).Length > 0;
    }

    public static void ShowLevels()
    {
        List<string> levels = FetchWorkInProgressLevels();

        levels = levels.OrderByDescending(x => DateTime.ParseExact(x, "HH:mm - dd / MMM / yyyy", CultureInfo.InvariantCulture)).ToList();

        DialogCanvas.instance.ShowLevelGaragePickerWindow(levels, (selectedIndex)=>
        {
            string levelToLoad = levels[selectedIndex];
            LoadWorkInProgress(levelToLoad);
        },
        (selectedIndex)=>
        {
            string levelToDelete = levels[selectedIndex];

            new Dialog(TranslationKey.Cloudsaving_Restore_LastChance_Title, TranslationKey.Generic_NoUndo)
                .AddNeutralButton(TranslationKey.Generic_No, ()=>{
                    //Do nothing.
                }, true)
                .AddDestructiveButton(TranslationKey.Generic_Yes, ()=>{
                    //Delete
                    DeleteWorkInProgressLevel(levelToDelete);
                    
                    DOVirtual.DelayedCall(Time.deltaTime, ()=>{
                        LevelBuilder.instance.CreateNewLevel();
                    });
                })
                .Show(true);
        });
    }

    public static void ShowGarage()
    {
        Dialog dialog = new Dialog(TranslationKey.LevelGarage_Header, TranslationKey.Cloudsaving_GooglePlayGames_Preregistration_Error_NoKey_Body);

        if(HasWorkInProgress())
            dialog.AddNeutralButton(TranslationKey.LevelGarage_LoadExistingLevel, () => {
                Garage.ShowLevels();
            });

        dialog.AddNeutralButton(TranslationKey.LevelGarage_CreateNewLevel, () =>
        {
            LevelBuilder.instance.CreateNewLevel();
        });

        dialog.Show();
    }

    private static List<string> FetchWorkInProgressLevels()
    {
        List<string> result = new List<string>();

        if(!Directory.Exists(directoryPath))
            return result;

        string[] files = Directory.GetFiles(directoryPath).Where(s => s.EndsWith(".wip", StringComparison.OrdinalIgnoreCase)).ToArray();

        foreach(string file in files)
        {
            string filename = Path.GetFileNameWithoutExtension(file);
            string levelName = WWW.UnEscapeURL(filename);
            result.Add(levelName);
        }

        return result;
    }

    public static bool CurrentWorkInProgressExsists(string levelName)
    {
        return File.Exists(GetWorkInProgressLevelPath(levelName));
    }

    public static void LoadWorkInProgress(string levelName, bool encrypted = true)
    {
        SaveManager.currentWorkInProgressName = levelName;
        string wip = GetWorkInProgress(levelName);
        
        if(encrypted)
            wip = StringCipher.Decrypt(wip, passPhrase);

        SerializableLevel level = LevelSerializer.DeserializeLevel(wip);
        GameMaster.instance.LoadLevel(level);
        LevelBuilder.instance.PlayMusic();
    }

    private static string GetWorkInProgress(string levelName)
    {
        string wip = "";
        
        if(!File.Exists(GetWorkInProgressLevelPath(levelName)))
        {
            Debug.LogError("Level not found!");
            return wip;
        }

        wip = File.ReadAllText(GetWorkInProgressLevelPath(levelName));

        return wip;
    }

    private static string savePathCache;
    private static string GetSavePath()
    {
        if(!string.IsNullOrWhiteSpace(savePathCache))
            return savePathCache;

        string path = "";
#if UNITY_ANDROID && !UNITY_EDITOR
        try {
                IntPtr obj_context = AndroidJNI.FindClass("android/content/ContextWrapper");
                IntPtr method_getFilesDir = AndroidJNIHelper.GetMethodID(obj_context, "getFilesDir", "()Ljava/io/File;");
        
                using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
                {
                    using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) 
                    {
                        IntPtr file = AndroidJNI.CallObjectMethod(obj_Activity.GetRawObject(), method_getFilesDir, new jvalue[0]);
                        IntPtr obj_file = AndroidJNI.FindClass("java/io/File");
                        IntPtr method_getAbsolutePath = AndroidJNIHelper.GetMethodID(obj_file, "getAbsolutePath", "()Ljava/lang/String;");   
                                        
                        path = AndroidJNI.CallStringMethod(file, method_getAbsolutePath, new jvalue[0]);                    
        
                        if(path != null) {
                            if(Debug.isDebugBuild) 
                                Debug.Log("Got internal path: " + path);
                        }
                        else {
                            if(Debug.isDebugBuild) 
                                Debug.Log("Using fallback path");

                            path = Application.persistentDataPath;
                        }
                    }
                }
            }
            catch(Exception e) {
                Debug.Log(e.ToString());
            }
#else
            path = Application.persistentDataPath;
#endif

        savePathCache = path;
        return savePathCache;
    }
}
