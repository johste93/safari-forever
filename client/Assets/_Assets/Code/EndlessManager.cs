using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using DG.Tweening;

public class EndlessManager : MonoBehaviour
{
    public LevelPreview preview;

    public int requiredUnlock = 24;
    public ScreenScroller screenScroller;
	public TextMeshProUGUI textMesh;

    public TextMeshProUGUI playerScoreTextMesh;

	public Image shadow;
	public Image surface;
	public Image padlock;
	public Image arrow;

    public Image background;
    public Image pattern;
    public Image leaderboardBackground;
    public Image leaderboardPattern;

    private EndlessChallengeResponse currentChallenge;
    
    private bool unlocked 
    {
        get { 
            if(Application.isEditor)
                return true;
                
            return GetNumberOfLevelsRequiredToUnlock() == 0; 
        }
    }

    private void Start()
	{
		surface.color = unlocked ? Color.white : Color.white.SetAlpha(0.6f);
		textMesh.color = LABColor.Lerp(shadow.color, surface.color, 0.5f).SetAlpha(0.6f);
		textMesh.text = GetNumberOfLevelsRequiredToUnlock().ToString();

		textMesh.gameObject.SetActive(!unlocked);
		padlock.gameObject.SetActive(!unlocked);
		arrow.gameObject.SetActive(unlocked);
	}

    public void ShowEndlessChallenge()
    {
        if(!unlocked)
		{
			Language messageLanguage = Localization.KeyAvailable(TranslationKey.Menu_WorldLocked_Message, SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;
			bool messageIsRTL = Localization.IsRightToLeftLanguage(messageLanguage);
			string message = Localization.GetTranslationFormat2(TranslationKey.Menu_WorldLocked_Message, SaveManager.currentSave.language, GetNumberOfLevelsRequiredToUnlock());
			
			new Dialog(
				TranslationKey.Menu_WorldLocked_Header, TranslationKey.Endless_Unlock)
			.AddNeutralButton(TranslationKey.Generic_Ok, null, true)
			.Show();
			return;
		}

        if(currentChallenge == null)
        {
            DialogCanvas.instance.ShowLoading();
            EndlessAPI.FetchChallenge((success, response)=>
            {
                DialogCanvas.instance.HideLoading();
                
                if(!success)
                    return;

                if(!HandleError(response.Error))
                {
                    currentChallenge = response;
                    LoadLevel(currentChallenge);
                }
                else
                {
                    currentChallenge = null;
                    preview.SetName("");
                    preview.SetImage(null);
                }

                if(response.PersonalScore >= 0)
                    playerScoreTextMesh.text = $"x{response.PersonalScore}";

                screenScroller.Next();
            });
        }
        else
        {
            screenScroller.Next();
        }
    }

    public void SkipChallenge()
    {
        if(currentChallenge == null)
        {
            preview.OnClick();
            return;
        }

        bool keyAvailable = Localization.KeyAvailable(TranslationKey.Endless_Reroll_Body, SaveManager.currentSave.language);
        Language keyLanguage = keyAvailable ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage;
        bool titleIsRTL = Localization.IsRightToLeftLanguage(keyLanguage);
        string formatedBody = Localization.GetTranslationFormat2(TranslationKey.Endless_Reroll_Body, keyLanguage, titleIsRTL ? $"<ltr>{currentChallenge.RerollCost}</ltr>" : currentChallenge.RerollCost.ToString() );

        new Dialog(TranslationKey.Endless_Reroll_Header, formatedBody, SaveManager.currentSave.language, Localization.IsRightToLeftLanguage(SaveManager.currentSave.language))
            .AddNegativeButton(TranslationKey.Generic_Cancel, null)
            .AddPositiveButton(TranslationKey.Generic_Yes, ()=>
            {
                DialogCanvas.instance.ShowLoading();
                EndlessAPI.SkipChallenge((success, response)=>
                {
                    DialogCanvas.instance.HideLoading();

                    if(!success)
                        return;

                    if(!HandleError(response.Error))
                    {
                        currentChallenge = response;
                        LoadLevel(currentChallenge);

                        SaveManager.currentSave.FetchOnlineProfile((profile)=>
                        {
                            profile.coins -= currentChallenge.RerollCost;
                            CurrencyButton.instance?.UpdateCoins(profile.coins);
                        });

                    }
                    else
                    {
                        currentChallenge = null;
                        preview.SetName("");
                        preview.SetImage(null);
                    }
                });
            })
            .Show();
    }

    public void Play()
    {
        if(currentChallenge == null)
        {
            preview.OnClick();
            return;
        }

        Level level = currentChallenge.ToLevel();

        GameVersion latestCompatibleVersion = new GameVersion(Globals.gameConstants.latestCompatibleVersion);
        GameVersion levelVersion = new GameVersion(level.PublishedLevelMeta.GameVersion);

        if(levelVersion.IsOlderThan(latestCompatibleVersion))
        {
            new Dialog(TranslationKey.Error_Outdated_Title, TranslationKey.Error_Outdated_Body)
                .AddNeutralButton(TranslationKey.Generic_Ok, null).Show();
            return;
        }

		GameVersion currentVersion = new GameVersion(Application.version);

        if (levelVersion.IsNewerThan(currentVersion))
        {
            new Dialog( TranslationKey.Error_UpdateGame_Title,TranslationKey.Error_UpdateGame_Body)
                .AddNeutralButton(TranslationKey.Generic_Ok, null).Show();
            return;
        }

        LevelAPI.CountPlay(level.PublishedLevelMeta.LevelId, level.PublishedLevelMeta.CreatorUserId, null);

        GlobalSingleton.exitTargetLocation = MenuLocation.EndlessMode;

        GlobalSingleton.windowIndex = 3;
        GlobalSingleton.levelToLoad = level;
        GlobalSingleton.mode = GameMode.FreePlay;
        GlobalSingleton.skipBlindsOpenOnNextSceneLoad = true;
        Camera.main.backgroundColor = level.serializableLevel.palette.main.color;
        SceneLoader.Load(SafariScene.Game);
    }

    private void LoadLevel(EndlessChallengeResponse endlessChallenge)
    {
        Level level = endlessChallenge.ToLevel();
        preview.SetName(level.PublishedLevelMeta.Name);
        FromBytes(endlessChallenge.thumbnail);

        background.DOKill();
        background.DOColor(level.serializableLevel.palette.main.color, 0.3f).OnUpdate(()=>{
            leaderboardBackground.color = background.color;
            pattern.color = background.color.SetVibrance(background.color.GetVibrance()-0.02f);
            leaderboardPattern.color = pattern.color;
        });
    }

    private void FromBytes(byte[] bytes)
	{
		Sprite sprite = bytes.ToSpriteFromGIF();

		if(sprite == null)
		{
			Debug.LogError("Unable to Parse Image");
			return;
		}

		preview.SetImage(sprite);
	}

    private int GetNumberOfLevelsRequiredToUnlock()
	{
		int beatenLevelsInThisWorld = 0;
        for(int i = 0; i < SaveManager.currentSave.campaignProgress.Length; i++)
            for(int j = 0; j < SaveManager.currentSave.campaignProgress[i].Length; j++)
                if(SaveManager.currentSave.campaignProgress[i][j].beaten)
                    beatenLevelsInThisWorld++;

		return Mathf.Max(requiredUnlock - beatenLevelsInThisWorld, 0);
	}

    private bool HandleError(EndlessError error)
    {
        switch(error)
        {
            case EndlessError.CantAffordToSkip:
                //Todo translate these errors.
                new Dialog(TranslationKey.Generic_Error, TranslationKey.Endless_Error_CantAffordSkip)
                    .AddNeutralButton(TranslationKey.Generic_Ok, ()=>
                    {
                        preview.OnClick();
                    }, true)
                    .Show();
            return true;
            case EndlessError.NoMoreLevels:
                //Todo translate these errors.
                new Dialog(TranslationKey.Generic_Error, TranslationKey.Endless_Error_NoMoreLevels)
                    .AddNeutralButton(TranslationKey.Generic_Ok, ()=>{
                        preview.OnClick();
                    }, true)
                    .Show();
            return true;
        }

        return false;
    }
}
