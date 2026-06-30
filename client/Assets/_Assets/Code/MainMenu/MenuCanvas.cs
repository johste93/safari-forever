using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuCanvas : MonoBehaviour
{
    public RectTransform[] rects;
    public CanvasGroup[] groups;

    public void Show(bool instant = false)
    {
        foreach(RectTransform rT in rects)
        {
            rT.DOKill();
            rT.DOAnchorPosY(0f, 0.3f).SetDelay(instant ? 0f : 0.4f);
        }

        foreach(CanvasGroup cG in groups)
        {
			cG.gameObject.SetActive(true);
			cG.blocksRaycasts = true;
            cG.interactable = true;
            cG.DOKill();
            cG.DOFade(1f, 0.3f).SetDelay(instant ? 0f : 0.4f);
        }
    }

	public void Hide(bool instant)
	{
        foreach(RectTransform rT in rects)
        {
            rT.DOKill();
            rT.DOAnchorPosY(50f, instant ? 0f : 0.3f);
        }

        foreach(CanvasGroup cG in groups)
        {
			cG.blocksRaycasts = false;
            cG.interactable = false;
            cG.DOKill();
            cG.DOFade(0f, instant ? 0f : 0.3f).OnComplete(()=>{
				cG.gameObject.SetActive(false);
			});
            
        }
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		foreach(RectTransform rT in rects)
            rT.DOKill();

		foreach(CanvasGroup cG in groups)
            cG.DOKill();
	}
}
