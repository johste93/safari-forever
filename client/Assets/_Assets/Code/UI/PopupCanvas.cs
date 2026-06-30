using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupCanvas : Singleton<PopupCanvas>
{
    public Canvas canvas;
    public RectTransform rectTransform;
    public GameObject container;
    public Image background;

    public PopupView profileView;
    public PopupView notificationView; 
    public PopupView levelsView; 
    public PopupView followingsView; 
    public PopupView summaryView;
    public PopupView inspectView; 
    public PopupView publishView; 
    public PopupView campaignSummaryView;
    public PopupView webSummaryView;
	public PopupView browserView;
    public PopupView notificationOptionsView;
    public PopupView rewardsView;
    public PopupView searchView;

    private List<Tween> tweens = new List<Tween>();

    public void EnableCanvas(bool instant = false, System.Action OnComplete = null)
    {
        if(container.activeInHierarchy)
        {
            OnComplete?.Invoke();
            return;
        }
           
        container.SetActive(true);

        //background.DOKill();
        background.color = background.color.SetAlpha(0f);
        tweens.Add(background.DOFade(1f, instant ? 0f : 0.3f ).OnComplete(()=>{
            OnComplete?.Invoke();
        }));
    }

    public void CloseCanvas(bool instant = false)
    {
        //background.DOKill();
        tweens.Add(background.DOFade(0f, instant ? 0f : 0.3f ).OnComplete(()=>{
            container.SetActive(false);
        }));

        profileView.Reset();
        notificationView.Reset();
        notificationOptionsView.Reset();
        levelsView.Reset();
        summaryView.Reset();
        inspectView.Reset();
        publishView.Reset();
        campaignSummaryView.Reset();
        webSummaryView.Reset();
		browserView.Reset();
        rewardsView.Reset();
        searchView.Reset();
        followingsView.Reset();
    }

    public void CloseCurrentView(bool instant, System.Action onComplete)
    {
        PopupView view = GetActivePopup();

        if(view != null)
            view.Close(instant, onComplete);
        else
            onComplete?.Invoke();
    }

    public bool HasActiveViews(PopupView excluded = null)
    {
        return  (profileView.gameObject.activeInHierarchy && excluded != profileView) ||
                (notificationView.gameObject.activeInHierarchy && excluded != notificationView) ||
                (levelsView.gameObject.activeInHierarchy && excluded != levelsView) ||
                (followingsView.gameObject.activeInHierarchy && excluded != followingsView) ||
                (summaryView.gameObject.activeInHierarchy && excluded != summaryView) ||
                (inspectView.gameObject.activeInHierarchy && excluded != inspectView) ||
                (publishView.gameObject.activeInHierarchy && excluded != publishView) ||
                (campaignSummaryView.gameObject.activeInHierarchy && excluded != campaignSummaryView) || 
                (webSummaryView.gameObject.activeInHierarchy && excluded != webSummaryView) ||
                (browserView.gameObject.activeInHierarchy && excluded != browserView) ||
                (notificationOptionsView.gameObject.activeInHierarchy && excluded != notificationOptionsView) ||
                (rewardsView.gameObject.activeInHierarchy && excluded != rewardsView) ||
                (searchView.gameObject.activeInHierarchy && excluded != searchView);
    }

    public bool IsShowing()
	{
		return container.activeInHierarchy;
	}

    private void OnDestroy()
	{
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		background.DOKill();
		
		foreach (Tween t in tweens)
        {
            t.Kill();
        }
        tweens = new List<Tween>();
	}

    public PopupView GetActivePopup()
    {
        PopupView view = container.transform.GetChild(container.transform.childCount-2).GetComponent<PopupView>();
        if(view.gameObject.activeInHierarchy)
            return view;
        
        return null;
    }
}
