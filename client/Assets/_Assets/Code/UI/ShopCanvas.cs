using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopCanvas : MonoBehaviour
{
	public Shop shop;
	public delegate void SpotlightAlpha(float alpha);
    public static SpotlightAlpha UpdateSpotlightAlpha;

    private RectTransform _thisRect;
	public RectTransform thisRect
	{
		get{
			if(_thisRect == null)
				_thisRect = GetComponent<RectTransform>();
			return _thisRect;
		}
	}

    private CanvasGroup _thisGroup;
	public CanvasGroup thisGroup
	{
		get{
			if(_thisGroup == null)
				_thisGroup = GetComponent<CanvasGroup>();
			return _thisGroup;
		}
	}

    public void Show(bool instant = false)
    {
		thisGroup.alpha = 0f;
		thisGroup.gameObject.SetActive(true);

		thisRect.DOKill();
        thisRect.DOAnchorPosY(50f, 0f);
		thisRect.DOAnchorPosY(0f, 0.3f).SetDelay(instant ? 0f : 0.4f);
		
		thisGroup.blocksRaycasts = true;
        thisGroup.interactable = true;

		thisGroup.DOKill();
		thisGroup.DOFade(1f, 0.3f).SetDelay(instant ? 0f : 0.4f).OnUpdate(()=>
		{
			if(UpdateSpotlightAlpha != null)
				UpdateSpotlightAlpha(thisGroup.alpha);
		});

		LayoutRebuilder.ForceRebuildLayoutImmediate(shop.randomCharacterOption);
		LayoutRebuilder.ForceRebuildLayoutImmediate(shop.randomHatOption);
    }

	public void Hide(bool instant = false)
	{
		thisRect.DOKill();
        thisRect.DOAnchorPosY(0f, 0f);
		thisRect.DOAnchorPosY(50f, instant ? 0f : 0.3f);

		thisGroup.blocksRaycasts = false;
        thisGroup.interactable = false;

		thisGroup.DOKill();
		thisGroup.DOFade(0f, instant ? 0f : 0.3f)
		.OnUpdate(()=>
		{
			if(UpdateSpotlightAlpha != null)
				UpdateSpotlightAlpha(thisGroup.alpha);
		}).OnComplete(()=>
		{
			thisGroup.gameObject.SetActive(false);
		});
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		thisRect.DOKill();
		thisGroup.DOKill();
	}
}
