using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CampaignSummary : PopupView
{
    public override void Show(System.Action onExit, bool instant = false, System.Action onComplete = null)
    {
        PopupCanvas.instance.background.DOKill();
        PopupCanvas.instance.background.DOColor(LevelBuilder.instance.GetCurrentColors().main, 0.3f);
        base.Show(onExit, instant, onComplete);
    }

    public void Replay()
    {
        base.Exit(false, ()=>{
            GameMaster.instance.ReplayLevel();
        });
    }

    public void Continue()
    {
        if(Stopwatch.instance != null)
            Stopwatch.instance.ResetAll();

        Level nextLevel = GetNextLevel();

        if(nextLevel == null)
        {
            BackToMenu();
            return;
        }

        LevelBuilder.instance.CreateNewLevel();
        
        GameMaster.instance.currentlyPlayingLevel = nextLevel;
        GameMaster.instance.LoadLevel(nextLevel.serializableLevel); 

        base.Exit();

        this.Delay1Frame(()=>
        {
            GameMaster.instance.PlayLevel();
            LevelBuilder.instance.PlayMusic();
            CameraScaler.instance.ZoomInOnLevel(true);
        });
    }

    public Level GetNextLevel()
    {
        int levelIndex = GameMaster.instance.currentlyPlayingLevel.campaignInfo.campaignIndex + 1;
		World world = GameMaster.instance.currentlyPlayingLevel.campaignInfo.world;
        string pathToLevel = $"Campaign/{world}/level_{levelIndex}";
        
        TextAsset tA = Resources.Load<TextAsset>(pathToLevel);
        if(tA == null)
        {
            Debug.LogError(pathToLevel + " not found!");
            return null;
        }
    
        string levelJson = tA.text;

       
        Level level = new Level(){
            serializableLevel = LevelSerializer.DeserializeLevel(levelJson),
            PublishedLevelMeta = new PublishedLevelMeta(),
            campaignInfo = new CampaignInfo(world, levelIndex)
        };

        return level;
    }

    public void BackToMenu()
    {
        //GO Streight to level overview
        SceneLoader.Load(SafariScene.Menu);
    }
}
