using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TermsOfServiceWindow : MonoBehaviour
{
	public RectTransform window;
	private System.Action<bool> onCallback;
    private Tween tween;

	public void OnClickAccept()
	{
		if(onCallback != null)
			onCallback(true);

		Close();
	}

	public void OnClickDecline()
	{
		if(onCallback != null)
			onCallback(false);

		Close();
	}

	public void OnClickReadTermsOfService()
	{
		Application.OpenURL("https://safariforever.com/terms");
	}

	public void Show(System.Action<bool> onCallback)
    {
		this.onCallback = onCallback;
		window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
		tween = window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);

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
}
