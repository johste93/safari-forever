using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelsView : PopupView
{
    public LevelsList levelsList;

    private string currentUserId;
    private BrowserTab tab;

    public void OnClickClose()
    {
        currentUserId = string.Empty;
        base.Exit();
    }

    public void ReloadLevels()
    {
        if(!string.IsNullOrEmpty(currentUserId))
            LoadUserLevels(currentUserId);
    }

    public void LoadUserLevels(string userId)
    {
        currentUserId = userId;

		levelsList.OnLoadMore = ()=>{
			FetchLevels(userId, 0, levelsList.GetCurrentIndex(), (success, response)=>
            {
                levelsList.scrollRect.enabled = true;

                if(!success)
                    return;

                HandleResponse(levelsList, response);
            });
		};

        levelsList.ClearLevels();
        FetchLevels(userId, 0, levelsList.GetCurrentIndex(), (success, response)=>
        {
            if(!success)
                return;

            HandleResponse(levelsList, response);
        });
    }

    /*
    public void SpawnLevels(string header, bool isRightToLeftText, List<LevelInfoDTO> levels, int levelsPrPage, MenuLocation exitLocation, bool showBoostButton)
    {
        
        levelsList.widgetHeader.isRightToLeftText = Localization.IsRightToLeftLanguage(SaveManager.currentSave.language);
        levelsList.widgetHeader.text = header;
        levelsList.levelsPrPage = levelsPrPage;

        levelsList.SpawnLevels(levels, exitLocation, showBoostButton);
    }
    */

    private void HandleResponse(LevelsList levelsList, LevelsByUserResponse response)
    {
        bool usedLanguageIsRightToLeft = Localization.IsRightToLeftLanguage(Localization.KeyAvailable(TranslationKey.LevelsWidget_Header, SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage);

        levelsList.widgetHeader.TranslateFormat(TranslationKey.LevelsWidget_Header, SaveManager.currentSave.language, FontType.Stylized_Outlined, false, usedLanguageIsRightToLeft ? $"<ltr>{response.Nickname}</ltr>" : response.Nickname);

        levelsList.levelsPrPage = response.LevelsPrPage;

        levelsList.SpawnLevels(response.Levels, MenuLocation.Profile, false);
    }

    private void FetchLevels(string userId, int count, int fromIndex, System.Action<bool, LevelsByUserResponse> onComplete = null)
    {
        DialogCanvas.instance.ShowLoading();

        LevelAPI.FetchUserLevels(userId, count, fromIndex, (success, result)=>
        {   
            DialogCanvas.instance.HideLoading();

            if(!success)
                Debug.LogError("Fetching levels failed.");

            onComplete?.Invoke(success, result);
        });
    }
}
