using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;

public class ReviewWindow : MonoBehaviour
{
    public RectTransform window;
    public GameObject[] slides;

    private int currentSlide = 0;
    private System.Action onComplete;
    private Tween tween;

    public void Show(System.Action onComplete)
    {
        if(SaveManager.hasAskedForReview)
		{
            onComplete?.Invoke();
            return;
        }

		SaveManager.hasAskedForReview = true;

        this.onComplete = onComplete;
        currentSlide = 0;

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
        Next();
    }

    public void Decline()
    {
        Close();
    }

    private void Next()
    {
        currentSlide++;

        if(currentSlide >= slides.Length)
        {
            OpenReviewWindow();
            Close();
            return;
        }

        SetSlide(currentSlide);

        LayoutRebuilder.ForceRebuildLayoutImmediate(window);
		LayoutRebuilder.ForceRebuildLayoutImmediate(window);
    }

    private void SetSlide(int slideIndex)
    {
        for(int i = 0; i < slides.Length; i++)
            slides[i].SetActive(i == slideIndex);
    }

    private static void OpenReviewWindow()
	{
#if UNITY_EDITOR
        return;
#endif

#if UNITY_IOS
		DeadMosquito.IosGoodies.IGAppStore.RequestReview();
#elif UNITY_ANDROID
		INNRate();
#endif
	}

    #if UNITY_IOS
	[DllImport ("__Internal")]
	private static extern void INNRate (string appId, bool openInAppstore);

	[DllImport ("__Internal")]
	private static extern string INNGetAppInfo ();
	#elif UNITY_ANDROID
	private static void INNRate() {
		AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");

		AndroidJavaObject jo = new AndroidJavaObject ("com.innocenttimes.plugins.ratemygame.RateMyGame");
		jo.CallStatic ("rate", new object[1] {activity});
	}

	private static string INNGetAppInfo() {
		AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");

		AndroidJavaObject jo = new AndroidJavaObject ("com.innocenttimes.plugins.ratemygame.RateMyGame");
		return jo.CallStatic<string>("getAppInfo", new object[1] {activity});
	}
	#endif
}
