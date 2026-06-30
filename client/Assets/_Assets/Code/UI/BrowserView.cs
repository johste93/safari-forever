using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BrowserView : PopupView
{
	public TextMeshProUGUI championTextmesh;
	public LevelElement levelOfTheDay;
	public LevelElement levelOfTheWeek;

	public GameObject featuredTab;
	public GameObject rankedTab;
	public GameObject boostTab;

	public LevelsList boostableLevels;

	public LevelsList boostedList;
	public RankedLevels rankedLevels;


	public BrowserCategory browserCategory;

	private bool featuredLoaded = false;
	private bool rankedLoaded = false;
	public bool boostedLoaded = false;
	public bool boostableLoaded = false;

	public bool showMyBoostableLevels = false;

	private void LoadFeaturedLevels(System.Action<bool> onComplete)
	{
		if(featuredLoaded)
		{
			onComplete?.Invoke(true);
			return;
		}

		levelOfTheDay.gameObject.SetActive(false);
		levelOfTheWeek.gameObject.SetActive(false);

		//Load level of the day
		DialogCanvas.instance.ShowLoading();
		BrowserAPI.FetchDailyChallenge((dailySuccess, dailyChallengeResponse) =>
		{
			if(dailySuccess)
			{
				levelOfTheDay.gameObject.SetActive(true);
				levelOfTheDay.Initalize(1, dailyChallengeResponse.Level, MenuLocation.Browser, false, false);

				championTextmesh.gameObject.SetActive(dailyChallengeResponse.HasChampion);
				championTextmesh.text = $"{dailyChallengeResponse.CurrentLeaderNickname}\n{dailyChallengeResponse.CurrentHighscore}";
			}
			/*
			else
			{
				DialogCanvas.instance.HideLoading();
				onComplete?.Invoke(false);
				return;
			}
			*/

			//Load Level of the week.
			BrowserAPI.FetchLevelOfTheWeek((weeklySuccess, levelOfTheWeekResponse) =>
			{
				if(weeklySuccess)
				{
					levelOfTheWeek.gameObject.SetActive(true);
					levelOfTheWeek.Initalize(1, levelOfTheWeekResponse.Level, MenuLocation.Browser, true, false);
				}
				/*
				else
				{
					DialogCanvas.instance.HideLoading();
					onComplete?.Invoke(false);
					return;
				}*/
				
				featuredLoaded = dailySuccess && weeklySuccess;
				DialogCanvas.instance.HideLoading();
				onComplete?.Invoke(featuredLoaded);
			}, true);
		}, true);
	}
	
	private void LoadRankedLevels(System.Action onComplete)
	{
		if(rankedLoaded)
		{
			onComplete?.Invoke();
			return;
		}

		rankedLevels.LoadLevels((success)=>{
			rankedLoaded = success;
		});
	}

	private void LoadBoostableLevels()
	{
		if(boostableLoaded)
			return;

		LoadLevelsForTab(BrowserTab.BoostableLevels);
	}

	public override void Show(System.Action onExit, bool instant = false, System.Action onComplete = null)
    {
		//newList.ClearLevels();
		//trendingList.ClearLevels();
		//popularList.ClearLevels();

		base.Show(onExit, instant, ()=>{
			
			SaveManager.currentSave.FetchOnlineProfile((profile)=>
			{
				if(profile == null)
					return;

				onComplete?.Invoke();
			});
		});
	}

	public void OnToggleClicked(bool isOn)
	{
		showMyBoostableLevels = isOn;

		if(showMyBoostableLevels)
		{
			SaveManager.currentSave.FetchOnlineProfile((profile)=>
			{
				if(profile == null)
					return;

				LoadBoostableLevels();
				boostedList.gameObject.SetActive(false);
				boostableLevels.gameObject.SetActive(true);

				boostableLevels.widgetHeader.Translate(TranslationKey.Browser_MyLevels, SaveManager.currentSave.language, FontType.Stylized_Outlined, false);
			});
			
		}
		else
		{
			boostableLevels.gameObject.SetActive(false);
			boostedList.gameObject.SetActive(true);

			boostedList.widgetHeader.Translate(TranslationKey.Browser_BoostedHeader, SaveManager.currentSave.language, FontType.Stylized_Outlined, false);
			

			if(!boostedLoaded)
			{
				((BrowserView)PopupCanvas.instance.browserView).LoadLevelsForTab(BrowserTab.Boosted);
			}
		}
	}

    public void OnClickClose()
    {
        base.Exit();
    }

	private void OnTabChanged(BrowserTab tab)
	{
		LoadLevelsForTab(tab);
	}

	public void LoadLevelsForTab(BrowserTab tab)
	{
		rankedTab.SetActive(tab == BrowserTab.Ranked);
		boostTab.SetActive(tab == BrowserTab.Boosted || tab == BrowserTab.BoostableLevels);
		featuredTab.SetActive(tab == BrowserTab.Featured);
		boostableLevels.gameObject.SetActive(tab == BrowserTab.BoostableLevels);
		boostedList.gameObject.SetActive(tab == BrowserTab.Boosted);

		switch(tab)
		{
			case BrowserTab.Featured:
				LoadFeaturedLevels(null);
			break;
			case BrowserTab.BoostableLevels:
				boostableLoaded = true;
				SelectLevelList(tab, boostableLevels);
			break;
			case BrowserTab.Boosted:
				boostedList.widgetHeader.Translate(TranslationKey.Browser_BoostedHeader, SaveManager.currentSave.language, FontType.Stylized_Outlined, false);
				boostedLoaded = true;
				SelectLevelList(tab, boostedList);
				OnToggleClicked(showMyBoostableLevels);
			break;
			case BrowserTab.Ranked:
				LoadRankedLevels(null);
			break;
		}
	}

	public void SelectLevelList(BrowserTab tab, LevelsList levelsList)
	{
		//On Load More
		levelsList.OnLoadMore = ()=>
		{
			DialogCanvas.instance.ShowLoading();

			System.Action<bool, LevelFeedRespone> callback = (success, levelFeed)=>
			{
				levelsList.scrollRect.enabled = true;

				DialogCanvas.instance.HideLoading();

				if(success)
				{
					levelsList.levelsPrPage = levelFeed.LevelsPrPage;

					MenuLocation exitLocation = MenuLocation.MainMenu;
					
					switch(tab)
					{
						case BrowserTab.PreviousDaily:
							exitLocation = MenuLocation.PreviousDaily; //Change
							break;
						case BrowserTab.PreviousWeekly:
							exitLocation = MenuLocation.PreviousWeekly; //Change
							break;

						case BrowserTab.Boosted:
							exitLocation = MenuLocation.Browser;
							break;

						case BrowserTab.BoostableLevels:
							exitLocation = MenuLocation.Boostable;
							break;

						case BrowserTab.New:
							exitLocation = MenuLocation.New;
							break;
						case BrowserTab.Popular:
							exitLocation = MenuLocation.Popular;
							break;
						case BrowserTab.Trending:
							exitLocation = MenuLocation.Trending;
							break;
					}

					levelsList.SpawnLevels(levelFeed.Levels, exitLocation, tab == BrowserTab.Boosted || tab == BrowserTab.BoostableLevels);

					if(tab == BrowserTab.BoostableLevels)
					{
						foreach(LevelElement element in boostableLevels.listElements)
							element.number.text = $"{element.GetLevel().BoostedRank}.";
					}
				}
			};

			switch(tab)
			{
				case BrowserTab.New:
					BrowserAPI.FetchNewLevels(levelsList.GetCurrentIndex(), 0, callback);	
				break;
				case BrowserTab.Boosted:
					BrowserAPI.FetchBoostedLevels(levelsList.GetCurrentIndex(), 0, callback);	
				break;
				case BrowserTab.Trending:
					BrowserAPI.FetchTrendingLevels(levelsList.GetCurrentIndex(), 0, callback);	
				break;
				case BrowserTab.Popular:
					BrowserAPI.FetchPopularLevels(levelsList.GetCurrentIndex(), 0, callback);	
				break;
				case BrowserTab.PreviousDaily:
					BrowserAPI.FetchPreviousDailyChallenges(levelsList.GetCurrentIndex(), 0, callback);	
				break;
				case BrowserTab.PreviousWeekly:
					BrowserAPI.FetchPreviousLevelsOfTheWeek(levelsList.GetCurrentIndex(), 0, callback);	
				break;
				case BrowserTab.BoostableLevels:
					BrowserAPI.FetchBoostableLevels(levelsList.GetCurrentIndex(), 0, callback);	
				break;
			}		
		};

		//If no levels loaded
		if(levelsList.container.childCount == 1)
		{
			levelsList.OnLoadMore?.Invoke();
		}
	}

	private void OnEnable()
	{
		browserCategory.OnTabChanged += OnTabChanged;
	}

	private void Unsubscribe()
	{
		browserCategory.OnTabChanged -= OnTabChanged;
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Unsubscribe();
	}
}
