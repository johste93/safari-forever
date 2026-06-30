using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupView : MonoBehaviour
{
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;

    protected List<Tween> tweens = new List<Tween>();

    protected const float delay = 0.1f;

    public System.Action onExit;

    public virtual void Show(System.Action onExit, bool instant = false, System.Action onComplete = null)
    {
        if(this.onExit == null && onExit != null)
            this.onExit = onExit;

        PopupCanvas.instance.EnableCanvas(instant, ()=>
        {
            rectTransform.SetSiblingIndex(rectTransform.parent.childCount-2);
            rectTransform.DOKill();
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 50f);

            tweens.Add(rectTransform.DOAnchorPosY(0f, instant ? 0f : 0.3f).SetDelay(instant ? 0f : delay));

            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            canvasGroup.gameObject.SetActive(true);

            canvasGroup.DOKill();
            canvasGroup.alpha = 0f;
            tweens.Add(canvasGroup.DOFade(1f, instant ? 0f : 0.3f).SetDelay(instant ? 0f : delay).OnComplete(()=>
            {
                onComplete?.Invoke();
            }));
        });
    }

    public virtual void Exit(bool instant = false, System.Action onComplete = null)
    {
        Close(instant, ()=>
        {
            onComplete?.Invoke();

            if(onExit != null)
            {
                onExit.Invoke();
                onExit = null;
            }
            else
            {
                PopupCanvas.instance.CloseCanvas();
            }
        });
    }

    public virtual void Close(bool instant = false, System.Action onComplete = null)
    {
        rectTransform.DOKill();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0f);

		tweens.Add(rectTransform.DOAnchorPosY(50f, instant ? 0f : 0.3f).SetDelay(instant ? 0f : delay));

		canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
		canvasGroup.DOKill();
        canvasGroup.alpha = 1f;
		tweens.Add(canvasGroup.DOFade(0f, instant ? 0f : 0.3f).SetDelay(instant ? 0f : delay).OnComplete(()=>
        {
            onComplete?.Invoke();
			canvasGroup.gameObject.SetActive(false);
        }));
    }

    public virtual void Reset()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0f;

        onExit = null;
    }

    protected virtual void OnDestroy()
	{
		KillAllTweens();
	}

	protected virtual void KillAllTweens()
	{
		foreach (Tween t in tweens)
        {
            t.Kill();
        }
        tweens = new List<Tween>();
	}
}
