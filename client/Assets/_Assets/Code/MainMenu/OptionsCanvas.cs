using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FSG.iOSKeychain;

public class OptionsCanvas : MonoBehaviour
{
	public GameObject deleteButton;
	public GameObject cloudsaveButton;

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

	public void UpdateButtonVisiblity()
	{
#if UNITY_ANDROID// && !UNITY_EDITOR
		AndroidCloudSave.HasBackup((success, hasBackup)=>{
			cloudsaveButton.SetActive(success && SaveManager.currentSave.HasOnlineProfile() && !hasBackup);
			//deleteButton.SetActive(!cloudsaveButton.activeInHierarchy);
		});
#else
		bool hasBackup = !string.IsNullOrWhiteSpace(SaveManager.RestoreToken);
        cloudsaveButton.SetActive(SaveManager.currentSave.HasOnlineProfile() && !hasBackup);
		//deleteButton.SetActive(!cloudsaveButton.activeInHierarchy);
#endif
	}

    public void Show(bool instant = false)
    {
		UpdateButtonVisiblity();
		
		thisRect.DOKill();
        thisRect.anchoredPosition = new Vector2(thisRect.anchoredPosition.x, 50f);
		thisRect.DOAnchorPosY(0f, instant ? 0f : 0.3f).SetDelay(instant ? 0f : 0.4f);
		
		thisGroup.alpha = 0f;
		thisGroup.gameObject.SetActive(true);
		thisGroup.blocksRaycasts = true;
        thisGroup.interactable = true;
		thisGroup.DOKill();
		thisGroup.DOFade(1f, instant ? 0f : 0.3f).SetDelay(instant ? 0f : 0.4f);
    }

	public void Hide(bool instant = false)
    {
		thisRect.DOKill();
        thisRect.anchoredPosition = new Vector2(thisRect.anchoredPosition.x, 0f);
		
		thisGroup.blocksRaycasts = false;
        thisGroup.interactable = false;
		thisRect.DOAnchorPosY(50f, instant ? 0f : 0.3f);
		thisGroup.DOKill();
		thisGroup.DOFade(0f, instant ? 0f : 0.3f).OnComplete(()=>
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
