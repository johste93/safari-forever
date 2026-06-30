using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuManager : Singleton<MenuManager>
{   
    public IntroView introView;
    public ScreenScroller screenScroller;
    public Carrossell carrossell;
	public ProfileButton profileButton;

    public BrowserCategory browserCategory;
    public PushToggle boostableToggle;
    public EndlessManager endlessManager;

    public Camera shopCamera;

    private Tween colorTween;

    public void Awake()
    {     
        System.Action onComplete = ()=>{
            MenuWebRequestManager.FetchUnrecivedRewards();
        };

        switch(GlobalSingleton.exitTargetLocation)
        {   
            default:
            case MenuLocation.MainMenu:
                introView.gameObject.SetActive(true);
                //onComplete?.Invoke();
            break;
            case MenuLocation.Campaign:
                ShowCampaignLevels(onComplete);
            break;
            case MenuLocation.EndlessMode:
                ShowEndlessMode(onComplete);
            break;
            case MenuLocation.Browser:
                ShowBrowser(onComplete);
            break;
            case MenuLocation.Boostable:
                ShowBoostable(onComplete);
            break;
            case MenuLocation.Profile:
                ShowProfile(onComplete);
            break;

            case MenuLocation.New:
                ShowLevelList(BrowserTab.New);
            break;
            case MenuLocation.Popular:
                ShowLevelList(BrowserTab.Popular);
            break;
            case MenuLocation.Trending:
                ShowLevelList(BrowserTab.Trending);
            break;

            case MenuLocation.PreviousDaily:
                ShowLevelList(BrowserTab.PreviousDaily);
            break;
            case MenuLocation.PreviousWeekly:
                ShowLevelList(BrowserTab.PreviousWeekly);
            break;
            
        }  

        GlobalSingleton.exitTargetLocation = MenuLocation.MainMenu;

#if UNITY_ANDROID
        if(AndroidCloudSave.playerHasPreviouslySignedIn)
            AndroidCloudSave.Authenticate(null);
#endif
    }

    private void Start()
    {
        MenuWebRequestManager.DoWebRequests();
    }
    
    public void ShowLevelList(BrowserTab browserTab)
    {
        MusicManager.Play(Music.Menu, true);
        shopCamera.gameObject.SetActive(true);

        SaveManager.currentSave.FetchOnlineProfile((profile)=>
		{
			if(profile == null)
				return;

            colorTween = PopupCanvas.instance.background.DOColor(Camera.main.backgroundColor, 0.3f);
            PopupCanvas.instance.browserView.onExit = ()=>{ PopupCanvas.instance.CloseCanvas(); };

            LevelsView levelsView =  (LevelsView)PopupCanvas.instance.levelsView;
            levelsView.levelsList.ClearLevels();

            switch(browserTab)
            {
                case BrowserTab.New:
                    levelsView.levelsList.widgetHeader.Translate(TranslationKey.Browser_NewHeader, SaveManager.currentSave.language, FontType.Stylized_Outlined, false);
                    break;
                case BrowserTab.Popular:
                    levelsView.levelsList.widgetHeader.Translate(TranslationKey.Browser_PopularHeader, SaveManager.currentSave.language, FontType.Stylized_Outlined, false);
                    break;
                case BrowserTab.Trending:
                    levelsView.levelsList.widgetHeader.Translate(TranslationKey.Browser_TrendingHeader, SaveManager.currentSave.language, FontType.Stylized_Outlined, false);
                    break;
                case BrowserTab.PreviousDaily:
                    levelsView.levelsList.widgetHeader.Translate(TranslationKey.Browser_DailyChallengeHeader, SaveManager.currentSave.language, FontType.Stylized_Outlined, false);
                    break;
                case BrowserTab.PreviousWeekly:
                    levelsView.levelsList.widgetHeader.Translate(TranslationKey.Browser_LevelOfTheWeekHeader, SaveManager.currentSave.language, FontType.Stylized_Outlined, false);
                    break;
            }

            ((BrowserView)PopupCanvas.instance.browserView).SelectLevelList(browserTab, levelsView.levelsList);

            levelsView.Show(()=>{
                PopupCanvas.instance.browserView.Show(null);
                this.DelayEndOfFrame(()=>{
                    browserCategory.SetCategory((BrowserTab) GlobalSingleton.browserIndex, true);
                    ((BrowserView)PopupCanvas.instance.browserView).LoadLevelsForTab((BrowserTab) GlobalSingleton.browserIndex);
                });
            });
		});
    }

    public void ShowCampaignLevels(System.Action onComplete)
    {
        MusicManager.Play(Music.Menu, true);
        shopCamera.gameObject.SetActive(true);
        carrossell.Hide();
        
        screenScroller.startIndex = GlobalSingleton.windowIndex;

        onComplete?.Invoke();
    }

    public void ShowEndlessMode(System.Action onComplete)
    {
        MusicManager.Play(Music.Menu, true);
        shopCamera.gameObject.SetActive(true);
        carrossell.Hide();
        
        screenScroller.startIndex = 2;

        endlessManager.ShowEndlessChallenge();

        onComplete?.Invoke();
    }

    public void ShowBrowser(System.Action onComplete)
    {
        MusicManager.Play(Music.Menu, true);
        shopCamera.gameObject.SetActive(true);
        
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
		{
			if(profile == null)
				return;

			colorTween = PopupCanvas.instance.background.DOColor(Camera.main.backgroundColor, 0.3f);
			PopupCanvas.instance.browserView.Show(()=>{
				PopupCanvas.instance.CloseCanvas();
			}, true);

            this.DelayEndOfFrame(()=>{
                browserCategory.SetCategory((BrowserTab) GlobalSingleton.browserIndex, true);
            
                ((BrowserView)PopupCanvas.instance.browserView).LoadLevelsForTab((BrowserTab) GlobalSingleton.browserIndex);

                this.Delay(0.5f, ()=>{
                    onComplete?.Invoke();
                });
            });
		});
    }

    public void ShowBoostable(System.Action onComplete)
    {
        MusicManager.Play(Music.Menu, true);
        shopCamera.gameObject.SetActive(true);
        
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
		{
			if(profile == null)
				return;

			colorTween = PopupCanvas.instance.background.DOColor(Camera.main.backgroundColor, 0.3f);
			PopupCanvas.instance.browserView.Show(()=>{
				PopupCanvas.instance.CloseCanvas();
			}, true);

            this.DelayEndOfFrame(()=>{
                browserCategory.SetCategory((BrowserTab) GlobalSingleton.browserIndex, true);
                ((BrowserView)PopupCanvas.instance.browserView).OnToggleClicked(true); //LoadLevelsForTab((BrowserTab) GlobalSingleton.windowIndex);
                boostableToggle.SetValue(true, true);

                this.Delay(0.5f, ()=>{
                    onComplete?.Invoke();
                });
            });
		});
    }

    public void ShowProfile(System.Action onComplete)
    {
        MusicManager.Play(Music.Menu, true);
        shopCamera.gameObject.SetActive(true);
        //carrossell.Hide();
        
        SaveManager.currentSave.FetchOnlineProfile((profile)=>
        {
            if(profile == null)
                return;

            DialogCanvas.instance.ShowLoading();
            ((ProfileView)PopupCanvas.instance.profileView).Initalize(GlobalSingleton.lastLoadedProfile, (success)=>
            {
                DialogCanvas.instance.HideLoading();

                if(!success)
                    return;

                PopupCanvas.instance.profileView.Show(()=>{
                    PopupCanvas.instance.CloseCanvas();
                });

                this.Delay(0.5f, ()=>{
                    onComplete?.Invoke();
                });
            });
        }); 
    }

    private void KillAllTweens()
    {
        colorTween?.Kill();
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }   

}
