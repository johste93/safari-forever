using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FSG.iOSKeychain;
using System;
using Chumpware.Security;

public class SaveManager
{
    private const string passPhrase = "vZ85Y%vigE*B6FID";
    
    private const string pPrefsKey = "Save";

    public const string RestoreTokenKey = "RestoreToken";
    public static string RestoreToken {
        get{
            return Keychain.GetValue(RestoreTokenKey);
        }
    }

    private static string savePathCache;

    private static Save _currentSave;
    public static Save currentSave
    {
        get{
            if(_currentSave == null)
                Load();
            
            return _currentSave;
        }
        set{
            _currentSave = value;
        }
    }

    public static string currentWorkInProgressName
    {
        get {
            return PlayerPrefs.GetString("CurrentWorkInProgressName");
        }
        set{
            PlayerPrefs.SetString("CurrentWorkInProgressName", value);
            PlayerPrefs.Save();
        }
    }

    public static bool firstBoot
    {
        get{
            return PlayerPrefs.GetString("FirstBoot", "true") == "true";
        }
        set{
            PlayerPrefs.SetString("FirstBoot", value.ToString().ToLower());
            PlayerPrefs.Save();
        }
    }

    public static bool discordAdHidden
    {
        get{
            return PlayerPrefs.GetString("DiscordAdHidden", "false") == "true";
        }
        set{
            PlayerPrefs.SetString("DiscordAdHidden", value.ToString().ToLower());
            PlayerPrefs.Save();
        }
    }

    public static bool analyticsConsentRequested
    {
        get{
            return PlayerPrefs.GetString("AnalyticsConsentRequested", "false") == "true";
        }
        set{
            PlayerPrefs.SetString("AnalyticsConsentRequested", value.ToString().ToLower());
            PlayerPrefs.Save();
        }
    }

    public static bool marketingConsentRequested
    {
        get{
            return PlayerPrefs.GetString("MarketingConsentRequested", "false") == "true";
        }
        set{
            PlayerPrefs.SetString("MarketingConsentRequested", value.ToString().ToLower());
            PlayerPrefs.Save();
        }
    }

    public static bool analyticsConsentGiven
    {
        get{
            return PlayerPrefs.GetString("AnalyticsConsentGiven", "false") == "true";
        }
        set{
            PlayerPrefs.SetString("AnalyticsConsentGiven", value.ToString().ToLower());
            PlayerPrefs.Save();
        }
    }

    public static bool marketingConsentGiven
    {
        get{
            return PlayerPrefs.GetString("MarketingConsentGiven", "false") == "true";
        }
        set{
            PlayerPrefs.SetString("MarketingConsentGiven", value.ToString().ToLower());
            PlayerPrefs.Save();
        }
    }

    public static bool hasAskedForReview
    {
        get{
            return PlayerPrefs.GetString("HasAskedForReview", "false") == "true";
        }
        set{
            PlayerPrefs.SetString("HasAskedForReview", value.ToString().ToLower());
            PlayerPrefs.Save();
        }
    }

    public static bool autoOrientationEnabled
    {
        get
        {
            return PlayerPrefs.GetString("AutoOrientationEnabled", "false") == "true";
        }
        set
        {
            PlayerPrefs.SetString("AutoOrientationEnabled", value.ToString().ToLower());
            PlayerPrefs.Save();
        }
    }

    public static Country? country
    {
        get{
            if(!PlayerPrefs.HasKey("Country"))
                return null;

            return (Country)PlayerPrefs.GetInt("Country");
        }
        set{
            if(value == null || !value.HasValue)
            {
                PlayerPrefs.DeleteKey("Country");
                return;
            }

            PlayerPrefs.SetInt("Country", (int) value.Value);
            PlayerPrefs.Save();
        }
    }

    public static int? age
    {
        get{
            if(!PlayerPrefs.HasKey("Age"))
                return null;

            return PlayerPrefs.GetInt("Age");
        }
        set{
            if(value == null || !value.HasValue)
            {
                PlayerPrefs.DeleteKey("Age");
                return;
            }

            PlayerPrefs.SetInt("Age", value.Value);
            PlayerPrefs.Save();
        }
    }

    public static void Save(bool encrypted = true)
    {
        if(Application.isEditor)
            Debug.Log("<color=Green>Saving!</color>");

        string json = JsonConvert.SerializeObject(currentSave);

        //Encrypt
        if(encrypted)
            json = StringCipher.Encrypt(json, passPhrase);

        PlayerPrefs.SetString(pPrefsKey, json);
        PlayerPrefs.Save();
    }   

    public static bool IsEncryped()
    {
        string data = PlayerPrefs.GetString(pPrefsKey);
        return data.Length > 0 && data[0] != '{';
    }
    
    public static void Load(bool encrypted = true)
    {
        if(Application.isEditor)
            Debug.Log("<color=Yellow>Loading Savegame</color>");

        if(!PlayerPrefs.HasKey(pPrefsKey))
        {
            CreateNewSave();
            return;
        }
        string json = PlayerPrefs.GetString(pPrefsKey);

        //Decrypt
        if(encrypted)
            json = StringCipher.Decrypt(json, passPhrase);

        _currentSave = (Save) JsonConvert.DeserializeObject<Save>(json);
        _currentSave.Migrate();
    }   

    private static void CreateNewSave()
    {
        if(Application.isEditor)
            Debug.Log("Creating new save!");

        _currentSave = new Save();
        _currentSave.saveVersion = new GameVersion(Application.version);
    }

    public static bool SaveExsists()
    {
        return PlayerPrefs.HasKey(pPrefsKey);
    }

    public static string GetSavePath()
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
