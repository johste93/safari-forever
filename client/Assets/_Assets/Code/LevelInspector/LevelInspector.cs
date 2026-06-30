using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LevelInspector : PopupView
{
    public Level level;

    public ShareCodeButton shareCodeButton;
    public LevelStatViewer levelStatViewer;
    public LevelPreview levelPreview;
    public CreatorButton creatorButton;

    private MenuLocation exitLocation;

    public void Initalize(Level level, MenuLocation exitLocation = MenuLocation.MainMenu)
    {
        this.exitLocation = exitLocation;

        levelStatViewer.Reset();
        levelPreview.Reset();
        this.level = level;
        
        levelPreview.SetName(level.PublishedLevelMeta.Name);

        creatorButton.SetCreatorUserId(level.PublishedLevelMeta.CreatorUserId);

        int averageNumberOfDeaths = Mathf.FloorToInt((float)level.PublishedLevelMeta.Deaths / (float)level.PublishedLevelMeta.Wins);

        shareCodeButton.shareUrl = level.PublishedLevelMeta.ShareUrl;

        

		//Load Thumbnail Async
        levelPreview.LoadImage(level.PublishedLevelMeta.LevelId, ()=>{
            levelStatViewer.PullStats(level.PublishedLevelMeta.LevelId, 0, (success)=>
            {
                /*
                BoostAPI.GetRank(level.PublishedLevelMeta.CoinsInvested, level.PublishedLevelMeta.CreatedOn, (getRankSuccess, response)=>
                {
                    if(!getRankSuccess)
                        return;

                    boostButton.SetActive(level.PublishedLevelMeta.CanBeBoosted && !level.PublishedLevelMeta.HasGraduated);
                    boostAmount.text = $"#{response.Rank}";
                    boostAmount.gameObject.SetActive(!level.PublishedLevelMeta.HasGraduated);
                }, true);
                */
            });
        });
    }

    public override void Show(System.Action onExit, bool instant = false, System.Action onComplete = null)
    {
        PopupCanvas.instance.background.DOKill();
        PopupCanvas.instance.background.DOColor(level.serializableLevel.palette.main.color, 0.3f);
        base.Show(onExit, instant, onComplete);
    }

    public void OnClickPlay()
    {
		GameVersion latestCompatibleVersion = new GameVersion(Globals.gameConstants.latestCompatibleVersion);
        GameVersion levelVersion = new GameVersion(level.PublishedLevelMeta.GameVersion);

        if(levelVersion.IsOlderThan(latestCompatibleVersion))
        {
            new Dialog(TranslationKey.Error_Outdated_Title, TranslationKey.Error_Outdated_Body)
                .AddNeutralButton(TranslationKey.Generic_Ok, () =>
                {
                    OnClickClose();
                }).Show();

            return;
        }

		GameVersion currentVersion = new GameVersion(Application.version);

        if (levelVersion.IsNewerThan(currentVersion))
        {
            new Dialog( TranslationKey.Error_UpdateGame_Title,TranslationKey.Error_UpdateGame_Body)
                .AddNeutralButton(TranslationKey.Generic_Ok, ()=> 
                {
                    OnClickClose();
                }).Show();
            return;
        }

        CountPlay();

        GlobalSingleton.exitTargetLocation = exitLocation;

        GlobalSingleton.levelToLoad = level;
        GlobalSingleton.mode = GameMode.FreePlay;
        GlobalSingleton.skipBlindsOpenOnNextSceneLoad = true;
        Camera.main.backgroundColor = level.serializableLevel.palette.main.color;
        SceneLoader.Load(SafariScene.Game);
    }

    public void OnClickClose()
    {
        base.Exit();
    }

    private void CountPlay()
    {
        if(level == null)
        {
            Debug.LogError("Level is null!");
            return;
        }
        
        LevelAPI.CountPlay(level.PublishedLevelMeta.LevelId, level.PublishedLevelMeta.CreatorUserId, null);
    }
}
