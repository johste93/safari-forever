using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Analytics;

public class AnalyticsWindow : MonoBehaviour
{
    private System.Action onComplete;
    public RectTransform window;
    private Tween tween;

	public void OnClickAccept()
	{
        ConsentManager.SetConsent(PrivacyConsentPurpose.Analytics, true);
		Close();

        onComplete?.Invoke();
	}

    public void OnClickAnonymous()
    {
        ConsentManager.SetConsent(PrivacyConsentPurpose.Analytics, true);
        Close();

        onComplete?.Invoke();
    }

	public void OnClickDecline()
	{
        ConsentManager.SetConsent(PrivacyConsentPurpose.Analytics, false);
		Close();

        onComplete?.Invoke();
	}

	public void Show(System.Action onComplete)
    {
        this.onComplete = onComplete;
        
		window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
        tween = window.DOAnchorPosY(100f, 0.3f);
        
        //gameObject.SetActive(true);
    }

	public void Close()
	{
		gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
	}

    private void OnDestroy()
    {
        KillAllTweens();
    }

    private void KillAllTweens()
    {
        if (tween != null)
        {
            tween.Kill();
            tween = null;
        }
    }

    public void OnClickReadAnalytics()
	{
#if ENABLE_CLOUD_SERVICES_ANALYTICS
        DataPrivacy.FetchPrivacyUrl(OpenUrl, OnFailure);
#endif
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
}

