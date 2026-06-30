using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSingleton
{
    public static ColorPalette colorPalette;
    public static bool loadEditor = true;
    public static bool isShuttingDown = false;
    public static bool skipBlindsOpenOnNextSceneLoad = false;

    public static Level levelToLoad;
#if UNITY_WEBGL
    public static GameMode mode = GameMode.FreePlay;
#else
    public static GameMode mode = GameMode.Create;
#endif

    //Used to remember players location when exiting level.
    public static MenuLocation exitTargetLocation = MenuLocation.MainMenu;
    public static int windowIndex = 0;
    public static int browserIndex = 0;
    public static string lastLoadedProfile;
    
}
