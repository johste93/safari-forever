using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AccessibilityCanvas : MonoBehaviour
{
	public GameObject openDyslexicGameObject;

    private bool isShowing;

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
        isShowing = true;

		openDyslexicGameObject.SetActive(SaveManager.currentSave.language != Language.Hebrew && SaveManager.currentSave.language != Language.Greek && SaveManager.currentSave.language != Language.Russian && SaveManager.currentSave.language != Language.Vietnamese && SaveManager.currentSave.language != Language.Serbian);
		thisGroup.alpha = 0f;
		thisGroup.gameObject.SetActive(true);

		thisRect.DOKill();
        thisRect.DOAnchorPosY(50f, 0f);
		thisRect.DOAnchorPosY(0f, 0.3f).SetDelay(instant ? 0f : 0.4f);

		thisGroup.blocksRaycasts = true;
        thisGroup.interactable = true;

		thisGroup.DOKill();
		thisGroup.DOFade(1f, 0.3f).SetDelay(instant ? 0f : 0.4f);
    }

	public void Hide(bool instant)
	{
		isShowing = false;

		thisRect.DOKill();
        thisRect.DOAnchorPosY(0f, 0f);
		thisRect.DOAnchorPosY(50f, 0.3f);

		thisGroup.blocksRaycasts = false;
        thisGroup.interactable = false;

		thisGroup.DOKill();
		thisGroup.DOFade(0f, 0.3f).OnComplete(()=>
		{
			thisGroup.gameObject.SetActive(false);
		});
	}

	private void KillAllTweens()
	{
		thisRect.DOKill();
		thisGroup.DOKill();
	}

	private void OnDestroy()
	{
		KillAllTweens();
	}
}
