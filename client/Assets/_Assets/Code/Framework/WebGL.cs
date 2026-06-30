using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WebGL : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_WEBGL
#if !UNITY_EDITOR
        string rawQueryString = Application.absoluteURL;

        Debug.LogError(rawQueryString);
#elif UNITY_EDITOR
         string rawQueryString = "https://api.safariforever.com/webgl/zyfEhN";
#endif
        string levelId = string.Empty;

		string[] segments = rawQueryString.Split('/');
		if(segments.Length != 5)
			return;

		levelId = segments[4];

		if(string.IsNullOrEmpty(levelId))
		{
            Debug.LogError("Unable to parse levelId from url: " + rawQueryString);
            return;
        }
         
        LevelAPI.DownloadNoVerify(levelId, (level)=>
		{
            if(level == null)
            {
                Debug.Log("Level not found");
                return;
            }

            GameVersion latestCompatibleVersion = new GameVersion(Globals.gameConstants.latestCompatibleVersion);
            GameVersion levelVersion = new GameVersion(level.PublishedLevelMeta.GameVersion);

            if(levelVersion.IsOlderThan(latestCompatibleVersion))
            {
                new Dialog(TranslationKey.Error_Outdated_Title, TranslationKey.Error_Outdated_Body).Show();
                return;
            }

            GameVersion currentVersion = new GameVersion(Application.version);

            if (levelVersion.IsNewerThan(currentVersion))
            {
                new Dialog( TranslationKey.Error_Outdated_Title, TranslationKey.Error_UpdateGame_Body).Show();
                return;
            }

            //CountPlay();
            GlobalSingleton.levelToLoad = level;
            Camera.main.backgroundColor = level.serializableLevel.palette.main.color;

            TransitionSingleton.instance.CloseBlinds(Camera.main.backgroundColor, true);
            GameMaster.instance.Initalize();
        });
#endif
    }
}
