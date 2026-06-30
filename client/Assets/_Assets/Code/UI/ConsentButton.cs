using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;

public class ConsentButton : MonoBehaviour
{
    public TextMeshProUGUI textMesh;

    private void OnEnable()
    {
        if(!SaveManager.age.HasValue || SaveManager.age < 18)
        {
            gameObject.SetActive(false);
            return;
        }

        if(CountryUtil.GDPRLawApplies(SaveManager.country.Value))
        {
            textMesh.Translate(TranslationKey.Menu_Consent_Button, SaveManager.currentSave.language, FontType.Regular, true);
        }
        else
        {
            textMesh.text = "Unity Analytics";
        }
        
        LanguageButton.On_LanguageChanged += On_LanguageChanged;
    }

    public void OnClick()
    {
        if(CountryUtil.GDPRLawApplies(SaveManager.country.Value))
        {
            DialogCanvas.instance.ShowConsentWindow(false, ()=>
            {
                /*
                //If we have consent, but FB not initalized yet, Initalise and Enable Facebook Tracking.
                if(ConsentManager.HasConsent(PrivacyConsentPurpose.Marketing))
                {
                    if(!FB.IsInitialized)
                    {
                        FacebookManager.Init((success)=>
                        {
                            if(!success)
                                return;

                            FacebookManager.EnableFacebookTracking();

                            FacebookManager.ActivateApp();
                        });
                    }
                }
                */
            });
        }
        else
        {
#if ENABLE_CLOUD_SERVICES_ANALYTICS
            DataPrivacy.FetchPrivacyUrl(OpenUrl, OnFailure);
#endif
        }
    }

#if ENABLE_CLOUD_SERVICES_ANALYTICS

    bool urlOpened = false;

    private void OpenUrl(string url)
    {
        urlOpened = true;

        Application.OpenURL(url);
    }

    private void OnFailure(string reason)
    {
        Debug.LogWarning(string.Format("Failed to get data privacy url: {0}", reason));

        new Dialog(TranslationKey.Generic_Error, string.Format("Failed to get data privacy url: {0}", reason), Globals.localizationConstants.defaultLanguage, false)
            .AddNeutralButton(TranslationKey.Generic_Ok, null)
            .Show();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && urlOpened)
        {
            urlOpened = false;
            // Immediately refresh the remote config so new privacy settings can be enabled
            // as soon as possible if they have changed.
            RemoteSettings.ForceUpdate();
        }
    }
#endif

    private void On_LanguageChanged(Language language)
    {
        if(CountryUtil.GDPRLawApplies(SaveManager.country.Value))
        {
            textMesh.Translate(TranslationKey.Menu_Consent_Button, language, FontType.Regular, true);
        }
    }

    private void Unsubscribe()
    {
        LanguageButton.On_LanguageChanged -= On_LanguageChanged;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}
