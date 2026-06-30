using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{   
    public delegate void SceneLoadEvent();
    public static event SceneLoadEvent On_BeforeSceneLoad;

    public static bool isLoadingScene;

    private static bool subscribed = false;

    public static void Load(SafariScene targetScene)
    {   
        Time.timeScale = 1f;
        isLoadingScene = true;
        if(!subscribed)
        {
            SceneManager.sceneLoaded += SceneLoaded;
            subscribed = true;
        }

        HandleMusic(targetScene);

        TransitionSingleton.instance.previousLevelColor = Camera.main.backgroundColor;
        //TransitionSingleton.instance.SetColor();

        if(On_BeforeSceneLoad != null)
            On_BeforeSceneLoad();

        TransitionSingleton.instance.CloseBlinds(TransitionSingleton.instance.previousLevelColor, false, ()=>{

            if(targetScene == SafariScene.Menu)
            {
                Color color = Color.black;
                
                switch(GlobalSingleton.exitTargetLocation)
                {
                    default:
                        //Yellow
                        color = new Color(1f, 0.7372886f, 0.2431372f, 1f);
                    break;
                    case MenuLocation.Campaign:
                    case MenuLocation.EndlessMode:
                    
                        switch (GlobalSingleton.windowIndex)
                        {
                            case 1:
                                //Blue
                                color = new Color(0.2980392f, 0.3843138f, 0.9019608f, 1f);
                            break;
                            case 2:
                            case 3:
                                //Green
                                color = new Color(0.7246074f, 1f, 0.359f, 1f);
                            break;
                        }
                    break;
                }


                TransitionSingleton.instance.FadeToColor(color, 0.3f, ()=>
                {
                    SceneManager.LoadScene(targetScene.ToString().ToUpper());
                    SuspensionManager.Suspend(false);
                });
            }
            else
            {
                SceneManager.LoadScene(targetScene.ToString().ToUpper());
                SuspensionManager.Suspend(false);
            }
        });
    }

    public static void Load(SafariScene targetScene, Color blinds)
    {   
        isLoadingScene = true;
        if(!subscribed)
        {
            SceneManager.sceneLoaded += SceneLoaded;
            subscribed = true;
        }

        HandleMusic(targetScene);

        TransitionSingleton.instance.previousLevelColor = blinds;
        //TransitionSingleton.instance.SetColor();

        if(On_BeforeSceneLoad != null)
            On_BeforeSceneLoad();

        TransitionSingleton.instance.CloseBlinds(blinds, false, ()=>{
            SceneManager.LoadScene(targetScene.ToString().ToUpper());
        });
    }

    private static void HandleMusic(SafariScene targetScene)
    {
        switch(targetScene)
        {
            case SafariScene.Menu:
                MusicManager.Play(Music.None, true);
            break;
            case SafariScene.Game:
                MusicManager.Play(Music.None, true);
            break;
        }
    }

    private static void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        isLoadingScene = false;
    }
}
