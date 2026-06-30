using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.SocialPlatforms;
using DG.Tweening;

public class Boot : MonoBehaviour
{   
    private void Start()
    {
        if(SaveManager.firstBoot)
        {
            SaveManager.firstBoot = false;
#if UNITY_ANDROID    
            //If we are playing on android we will try to login on the very first boot to simplefy registration later.
            AndroidCloudSave.Authenticate(null);
#endif
        }

        Step1();
    }

	private void Step1() //Migration
    {
        MigrationManager.Migrate(()=>
        {
			MusicCollection.Preload();

			WebWorker worker = WebWorkerFactory.HireWorker();
			worker.CheckInternetConnection(true, (isConnected, serverTime)=>
			{
                if(!isConnected)
                {
                    Step2();
                    return;
                }

                bool failSilently = false;
                Client.VerifyVersion(failSilently, (success, isUpToDate)=>
                {
                    if(success && !isUpToDate)
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif 
                        return;
                    }
                    else
                    {
                        Step2();
                    }
                });
			});
		});
    }

    private void Step2()
    {
        //Age verification
        if(!SaveManager.age.HasValue)
        {
            DialogCanvas.instance.ShowAgeVerification((success, age)=>
            {
                if(!success)
                {
                    Step2();
                    return;
                }

                SaveManager.age = age;

                Step3();
            }, false);
        }
        else
        {
            Step3();
        }
    }

    private void Step3()
    {
        if(!SaveManager.country.HasValue)
        {
            DialogCanvas.instance.ShowCountryWindow((country)=>
            {
                SaveManager.country = country;

                Language languageClosestToDeviceLanguage = Language.English;
                if(Globals.localizationConstants.LanguageIsEnabled(languageClosestToDeviceLanguage))
                    SaveManager.currentSave.language = languageClosestToDeviceLanguage;
                else
                    SaveManager.currentSave.language = Globals.localizationConstants.defaultLanguage;
                
                SaveManager.Save();

                Step5();
            });
        }
        else
        {
            Step5();
        }
    }

    private void Step4()
    {
        //Dont ask users under 18 for consent. We dont track kids.
        if(SaveManager.age < 18)
        {
            Step5();
            return;
        }

        DialogCanvas.instance.ShowConsentWindow(true, ()=>
        {
            //Done asking for consent.

#if ENABLE_CLOUD_SERVICES_ANALYTICS
            //Enable Unity Analytics
            if(SaveManager.analyticsConsentRequested && SaveManager.analyticsConsentGiven)
                DataPrivacyOptInFlow.UserHasOptedIntoLimitedDataCollection_LetsResumeAnalyticsInitialization();
#endif

            Step5();
        });
    }

    private void Step5()
    {
        if(DialogCanvas.instance.backgroundCanvasGroup.gameObject.activeInHierarchy)
        {
            DOVirtual.DelayedCall(0.3f, ()=>{
                Step6();
            });
        }
        else
        {
            Step6();
        }
    }

    private void Step6()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    private Dictionary<string, string> ParseQuery(string s)
    {
         var args = new Dictionary<string, string>();
            try
            {
                var parser = new UrlEncodingParser(s);
                args = parser;
            }
            catch(Exception e)
            {
                Debug.Log(e.ToString());
            }
        
        return args;
    }
}
