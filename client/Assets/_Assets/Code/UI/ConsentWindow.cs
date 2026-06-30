using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Analytics;

public class ConsentWindow : MonoBehaviour
{
    public RectTransform window;
    public GameObject[] slides;

    private int currentSlide = 0;
    private System.Action onComplete;
    private Tween tween;

    public void Show(bool skipIfHasConsent, System.Action onComplete)
    {
        this.onComplete = onComplete;
        currentSlide = 0;

        //If user lives outside of GDPR area we dont have to ask for consent.
        if(!CountryUtil.GDPRLawApplies(SaveManager.country.Value))
        {
            Close();
            return;
        }

        if(skipIfHasConsent)
        {
//Skip slides we have consent for.
//If analytics not enabled skip first slide.
#if ENABLE_CLOUD_SERVICES_ANALYTICS

            if(SaveManager.analyticsConsentRequested)
                currentSlide++;
#else
            currentSlide = 1;
#endif
            if(SaveManager.marketingConsentRequested)
                currentSlide++;
        
            //If we have consent for all slides close window.
            if(currentSlide >= slides.Length)
            {
                Close();
                return;
            }
        }
        
        SetSlide(currentSlide);
        
        DialogCanvas.instance.FadeInBackground();

		window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
        tween = window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);
    }

    private void Close()
    {
        gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
        onComplete?.Invoke();
    }

    public void Accept()
    {
        ConsentManager.SetConsent((PrivacyConsentPurpose) currentSlide, true);
        Next();
    }

    public void Decline()
    {
        ConsentManager.SetConsent((PrivacyConsentPurpose) currentSlide, false);
        Next();
    }

    public void AboutAnalytics()
	{
#if ENABLE_CLOUD_SERVICES_ANALYTICS
        DataPrivacy.FetchPrivacyUrl(OpenUrl, OnFailure);
#endif
	}

    public void AboutMarketing()
	{
        Application.OpenURL("https:safariforever.com/privacy");
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

    private void Next()
    {
        currentSlide++;

        if(currentSlide >= slides.Length)
        {
            Close();
            return;
        }

        SetSlide(currentSlide);
    }

    private void SetSlide(int slideIndex)
    {
        for(int i = 0; i < slides.Length; i++)
            slides[i].SetActive(i == slideIndex);
    }
}
