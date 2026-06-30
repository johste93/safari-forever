using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;
using System.Globalization;
using System;

public class LoadLevelButton : MonoBehaviour
{
    public World world;

    public LevelStats stats;
	public GameObject beatenContainer;
	public TextMeshProUGUI smallLevelIndexTextMesh;

    public TextMeshProUGUI textMesh;
    public Image padlock; 
	public TextMeshProUGUI padlockRemaining; 
    public Image playIcon;

    public Image lockedSurface;
	public Image shadow;

    public delegate void LoadLevelButtonEvent(LoadLevelButton btn);
    public static LoadLevelButtonEvent On_LoadLevelButtonClicked;

    private int timesClicked = 0;
    public int levelIndex = 0;
    private bool highscoreHasLoaded;
	private string pathToLevel {
		get{ return $"Campaign/{world}/level_{levelIndex}";}
	}

    private bool unlocked{
        get{
            if(Application.isEditor)
                return true;

			switch(world)
			{
				case World.World_1:
					if(levelIndex < 3)
					{
						//First 3 levels need to be played in order.
						return GetNumberOfLevelsBeaten(World.World_1) >= levelIndex;
					}
					else
					{
						//The last 9 unlocks once the tree first are beaten.
						return GetNumberOfLevelsBeaten(World.World_1) >= 3;
					}
				case World.World_2:
					//need to beat alteast 9 levels before world 2 unlocks!
					return GetNumberOfLevelsBeaten(World.World_1) >= 9;
			}
			return false;
        }
    }

	private int GetTotalNumberOfLevelsToUnlock()
	{
		switch(world)
		{
			case World.World_1:
				if(levelIndex < 3)
				{
					//First 3 levels need to be played in order.
					return levelIndex;
				}
				else
				{
					//The last 9 unlocks once the tree first are beaten.
					return 3;
				}
			case World.World_2:
				return 9;
			case World.World_3:
				return 18;
		}
		return -1;
	}

	private int GetNumberOfLevelsBeaten(World w)
	{
		return SaveManager.currentSave.campaignProgress[(int)w].Count(x => x.beaten == true);
	}

	private int GetNumberOfLevelsBeatenTotal()
	{
		int result = 0;
		for(int i = 0; i < SaveManager.currentSave.campaignProgress.Length; i++)
			result += GetNumberOfLevelsBeaten((World)i);
		return result;
	}

	private int GetNumberOfLevelsRequiredToUnlock()
	{
		return Mathf.Max(GetTotalNumberOfLevelsToUnlock() -  GetNumberOfLevelsBeatenTotal(), 0);
	}

    public void OnClick()
    {
        timesClicked++;

        if(On_LoadLevelButtonClicked != null)
            On_LoadLevelButtonClicked(this);

        if(!unlocked)
		{
            Language messageLanguage = Localization.KeyAvailable(TranslationKey.Menu_LevelLocked_Message, SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;
            bool messageIsRTL = Localization.IsRightToLeftLanguage(messageLanguage);
			string message = Localization.GetTranslationFormat2(TranslationKey.Menu_LevelLocked_Message, SaveManager.currentSave.language, GetNumberOfLevelsRequiredToUnlock());

			new Dialog(
                TranslationKey.Menu_LevelLocked_Header,
                message, messageLanguage,
                messageIsRTL)
			.AddNeutralButton(TranslationKey.Generic_Ok, null, true)
			.Show();

			return;
		}
        
        if(timesClicked == 1)
        {
            LoadHighscore();
            UpdateButtonSurface();

            return;
        }

        LoadLevel();
    }

    private void UpdateButtonSurface()
    {
		bool levelBeaten = SaveManager.currentSave.campaignProgress[(int)world][levelIndex].beaten;
		beatenContainer.SetActive(timesClicked == 0 && levelBeaten);
		smallLevelIndexTextMesh.color = textMesh.color.SetAlpha(levelBeaten ? 0.2f : 1f);

        lockedSurface.color = unlocked ? Color.white : Color.white.SetAlpha(0.6f);
        padlock.gameObject.SetActive(!unlocked);
        textMesh.gameObject.SetActive(timesClicked == 0 && unlocked && !levelBeaten);
        playIcon.gameObject.SetActive(timesClicked == 1 && unlocked);
		
		padlockRemaining.text = GetNumberOfLevelsRequiredToUnlock().ToString();

		padlockRemaining.color = LABColor.Lerp(shadow.color, lockedSurface.color, 0.5f).SetAlpha(0.6f);
    }

	private bool LevelExsists()
	{
        TextAsset tA = Resources.Load<TextAsset>(pathToLevel);
		return tA != null;
	}

    private void LoadLevel()
    {   
        if(!LevelExsists())
        {
            Debug.LogError($"{world}/level_{levelIndex} not found!");
            new Dialog("404 ;)", false, Globals.localizationConstants.defaultLanguage, "Level does not exsist yet! Try Level Builder instead!", false, Globals.localizationConstants.defaultLanguage)
                .AddNeutralButton(TranslationKey.Generic_Ok, null, true)
                .Show();
            return;
        }

        TextAsset tA = Resources.Load<TextAsset>(pathToLevel);
        string levelJson = tA.text;

       
        Level level = new Level(){
            serializableLevel = LevelSerializer.DeserializeLevel(levelJson),
            PublishedLevelMeta = new PublishedLevelMeta(),
            campaignInfo = new CampaignInfo(world,levelIndex)
        };

        bool outdated = string.IsNullOrWhiteSpace(level.serializableLevel.gameVersion);

        if(!outdated)
        {
            GameVersion latestCompatibleVersion = new GameVersion(Globals.gameConstants.latestCompatibleVersion);
            GameVersion levelVersion = new GameVersion(level.serializableLevel.gameVersion);
            if (levelVersion.IsOlderThan(latestCompatibleVersion))
                outdated = true;
        }
        
        if(outdated)
        {
            new Dialog(TranslationKey.Error_Outdated_Title, TranslationKey.Error_Outdated_Body)
                .AddNeutralButton(TranslationKey.Generic_Ok, null).Show();

            return;
        }

        GlobalSingleton.levelToLoad = level;

        ColorPalette colorPalette = (ColorPalette) ScriptableObject.CreateInstance(typeof(ColorPalette));
        colorPalette.Deserialize(level.serializableLevel.palette);
        GlobalSingleton.colorPalette = colorPalette;

        GlobalSingleton.exitTargetLocation = MenuLocation.Campaign;
        GlobalSingleton.windowIndex = (int) level.campaignInfo.world+1;

        GlobalSingleton.mode = GameMode.Campaign;
        GlobalSingleton.skipBlindsOpenOnNextSceneLoad = true;
        SceneLoader.Load(SafariScene.Game, GlobalSingleton.colorPalette.main);
    }

    private void OnLoadLevelButtonClicked(LoadLevelButton btn)
    {
        if(btn != this)
        {
            timesClicked = 0;
            UpdateButtonSurface();
        }

        if(timesClicked >= 1)
        {
			if(!unlocked)
				return;

            if(timesClicked == 1)
            {
                stats.DOKill();
                stats.rectTransform.localScale = new Vector3(0,0,1);
                stats.gameObject.SetActive(true);
                stats.rectTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
            }
        }
        else
        {
            if(stats.gameObject.activeInHierarchy)
                stats.rectTransform.DOScale(0f, 0.2f).SetEase(Ease.OutQuad).OnComplete(()=>{
                    stats.gameObject.SetActive(false);
                }); 
        }
    }

    private void OnScreenChange()
    {
        OnLoadLevelButtonClicked(null);
    }

    private void OnMergeComplete()
	{
		UpdateButtonSurface();
	}

    private void OnEnable()
    {
        Highscore localHighscore = new Highscore(SaveManager.currentSave.campaignProgress[(int)world][levelIndex].seconds, SaveManager.currentSave.campaignProgress[(int)world][levelIndex].milliseconds);
          
		if(localHighscore.Seconds > 0)
		{
			stats.SetLocalHeighScore(localHighscore.ToString());  
		}		
        else
        {
            stats.SetLocalHeighScore("--:--");
        }

        int levelNumber = ((int)world * 12) + levelIndex+1;
        textMesh.text = levelNumber.ToString();
		smallLevelIndexTextMesh.text = textMesh.text;
        UpdateButtonSurface();        
        
        ScreenScroller.OnScreenChange += OnScreenChange;
        LoadLevelButton.On_LoadLevelButtonClicked += OnLoadLevelButtonClicked;
        Save.OnMergeComplete += OnMergeComplete;
    }

    private void Unsubscribe()
    {
        ScreenScroller.OnScreenChange -= OnScreenChange;
        LoadLevelButton.On_LoadLevelButtonClicked -= OnLoadLevelButtonClicked;
        Save.OnMergeComplete -= OnMergeComplete;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
		KillAllTweens();
    }

	private void KillAllTweens()
	{
		stats.DOKill();
		stats.rectTransform.DOKill();
	}

    private void LoadHighscore()
    {
        if(highscoreHasLoaded)
            return;

        highscoreHasLoaded = true;

        HighscoreAPI.GetHighscore((int)world, levelIndex, (success, score)=>
        {
            highscoreHasLoaded = success;

            if(!success)
                return;

            if(score < 0)
            {
                stats.SetWorldHeighScore("--:--");
            }
            else
            {
                stats.SetWorldHeighScore(Stopwatch.ToString(score));
            }
        });
    }
}
