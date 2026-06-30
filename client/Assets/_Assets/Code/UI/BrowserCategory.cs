using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BrowserCategory : MonoBehaviour
{
	public RectTransform highlight;

	public RectTransform featuredTab;
	public RectTransform newTab;
	public RectTransform boostedTab;
	public RectTransform trendingTab;
	public RectTransform popularTab;
	public RectTransform rankedTab;

	private float duration = 0.25f;
	private BrowserTab currentTab;

	public delegate void BrowserCategoryEvent(BrowserTab tab);
	public BrowserCategoryEvent OnTabChanged;

	public void SetCategory(BrowserTab tab, bool instant = false)
	{
		if(tab == currentTab)
			return;

        GlobalSingleton.browserIndex = (int) tab;

		highlight.DOKill();
		switch(tab)
		{
			case BrowserTab.Featured:
				highlight.DOAnchorPos(featuredTab.anchoredPosition, instant ? 0f : duration);
			break;
			case BrowserTab.New:
				highlight.DOAnchorPos(newTab.anchoredPosition, instant ? 0f : duration);
			break;
			case BrowserTab.Trending:
				highlight.DOAnchorPos(trendingTab.anchoredPosition, instant ? 0f : duration);
			break;
			case BrowserTab.Popular:
				highlight.DOAnchorPos(popularTab.anchoredPosition, instant ? 0f : duration);
			break;
			case BrowserTab.Boosted:
				highlight.DOAnchorPos(boostedTab.anchoredPosition, instant ? 0f : duration);
			break;
			case BrowserTab.Ranked:
				highlight.DOAnchorPos(rankedTab.anchoredPosition, instant ? 0f : duration);
			break;
		}

		if(!instant)
			Audio.Play(SFX.instance.ui.tabChange, Channel.UI).SetPitch(Random.Range(0.9f, 1.1f));

		currentTab = tab;
	}

	public void OnClickFeatured()
	{
		SetCategory(BrowserTab.Featured);
		OnTabChanged?.Invoke(BrowserTab.Featured);
	}

    public void OnClickNew()
	{
		SetCategory(BrowserTab.New);
		OnTabChanged?.Invoke(BrowserTab.New);
	}

	public void OnClickTrending()
	{
		SetCategory(BrowserTab.Trending);
		OnTabChanged?.Invoke(BrowserTab.Trending);
	}

	public void OnClickPopular()
	{
		SetCategory(BrowserTab.Popular);
		OnTabChanged?.Invoke(BrowserTab.Popular);
	}

	public void OnClickBoosted()
	{
		SetCategory(BrowserTab.Boosted);
		OnTabChanged?.Invoke(BrowserTab.Boosted);
	}

	public void OnClickRanked()
	{
		SetCategory(BrowserTab.Ranked);
		OnTabChanged?.Invoke(BrowserTab.Ranked);
	}

	private void OnDestroy()
	{
		highlight.DOKill();
	}
}
