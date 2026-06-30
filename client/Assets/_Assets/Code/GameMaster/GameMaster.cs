using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM_CharacterController2D;
using System.Linq;

public class GameMaster : Singleton<GameMaster>
{
    public GameObject editorCanvasPrefab;

    private bool isPlaying;
    private bool hasStartedRunning;
    private bool isPublishingLevel;
    private int roomsCleared;
    private bool transitionPointOfNoReturnReached;

    private int startFrame;

    public static GameEvent On_EnterPlayMode;
    public static GameEvent On_ExitPlayMode;
    public static PlayerGameEvent On_PlayerStartedRunning;
    public static PlayerGameEvent On_PlayerDied;
    public static PlayerGameEvent On_PlayerWon;

    public delegate void ResetEvent(bool manual);
    public static ResetEvent On_LevelReset;


    public Level currentlyPlayingLevel;
    private LevelUserStats levelUserStats;

    private int[] jumps;
    private int deaths;

    private void Awake()
    {
#if !UNITY_WEBGL
        Initalize();
#endif
    }

    public void Initalize()
    {
        deaths = 0;
        jumps = new int[4];

        if(GlobalSingleton.mode == GameMode.Create)
        {
#if !UNITY_WEBGL
            Instantiate(editorCanvasPrefab);
            LogicCanvas.instance.backgroundRect.gameObject.SetActive(true);
            RailCanvas.instance?.backgroundRect.gameObject.SetActive(true);
            
            if(Garage.CurrentWorkInProgressExsists(SaveManager.currentWorkInProgressName))
            {
                new Dialog(TranslationKey.Editor_WorkInProgress_Header, TranslationKey.Editor_WorkInProgress_Body)
                .AddNeutralButton(TranslationKey.Editor_WorkInProgress_StartOver, ()=>
                {
                    SaveManager.currentWorkInProgressName = "";
                    LevelBuilder.instance.PlayMusic();
                })
                .AddPositiveButton(TranslationKey.Editor_WorkInProgress_Continue, ()=>{
                    Garage.LoadWorkInProgress(SaveManager.currentWorkInProgressName);
                    LevelBuilder.instance.PlayMusic();
                    //Garage.ShowLevels();
                }, true)
                .Show(true);
            }
            else
            {
                this.Delay1Frame(()=>
                {
                    LevelBuilder.instance.PlayMusic(false);
                });
            }
#endif
        }
        else
        {
            currentlyPlayingLevel = GlobalSingleton.levelToLoad;
 
            this.Delay1Frame(()=>
            {
                LoadLevel(currentlyPlayingLevel.serializableLevel);
                GlobalSingleton.levelToLoad = null;

                float duration = 0.5f;
                string text = "";
                switch(GlobalSingleton.mode)
                {
                    case GameMode.Campaign:
                        int levelNumber = ((int)currentlyPlayingLevel.campaignInfo.world * 12) + currentlyPlayingLevel.campaignInfo.campaignIndex + 1;
                        text = $"{levelNumber} - 1";
                        duration = 0.5f;
                        break;
                    case GameMode.FreePlay:
                        string levelName = currentlyPlayingLevel.PublishedLevelMeta.Name;
                        string creator = currentlyPlayingLevel.PublishedLevelMeta.CreatorUserName.Split('#')[0];

                        if(Localization.IsRightToLeftLanguage(SaveManager.currentSave.language))
                        {
                            if(Localization.KeyAvailable(TranslationKey.Transition_LevelBy, SaveManager.currentSave.language))
                            {
                                levelName = Localization.ReverseString(levelName);
                                creator = Localization.ReverseString(creator);
                            }
                        }  

                        text = Localization.GetTranslation2(TranslationKey.Transition_LevelBy, SaveManager.currentSave.language);
                        text = text.Replace("{LevelName}", levelName);
                        text = text.Replace("{CreatorNickname}", creator);
                        duration = 1f;
                        break;
                }
                
                //Replace language with actual languague when localized "by: user".
                TransitionSingleton.instance.ShowFlyByText(text, SaveManager.currentSave.language, Localization.IsRightToLeftLanguage(SaveManager.currentSave.language), duration, false, null, () =>
                {
                    LoadLevel(currentlyPlayingLevel.serializableLevel);
                    GlobalSingleton.levelToLoad = null;
                    

                    this.Delay1Frame(() =>
                    {
                        PlayLevel();
                        LevelBuilder.instance.PlayMusic();
                        CameraScaler.instance.ZoomInOnLevel(true);
                        TransitionSingleton.instance.OpenBlinds();

						if(Application.isEditor && Globals.debugConstants.saveLevelsPlayedAsWIP)
						{
                            Garage.SaveWorkInProgressLevel(currentlyPlayingLevel.PublishedLevelMeta.Name);
						}

                        if(GlobalSingleton.mode == GameMode.FreePlay)
                            GetUserStats(null);
                    });
                });
            });
        }  
    }

    public void StartRunning(FSM_CharacterController controller)
    {
        hasStartedRunning = true;

        if(On_PlayerStartedRunning != null)
            On_PlayerStartedRunning(controller);
    }

    public void PlayerDied(FSM_CharacterController controller, bool instagibbed, bool invalidSpawn)
    {
        deaths++;

        Time.timeScale = 0.5f;
        if (MainCamera.instance != null)
            MainCamera.instance.PlayDeathEffect(() =>
            {
                Time.timeScale = 1f;
                TransitionSingleton.instance.OpenBlinds();

                

                if ((instagibbed && !IsAttemptingToPublishLevel()) || invalidSpawn)
                    ExitPlayMode();
                else
                    ResetLevel();
            });

        if (On_PlayerDied != null)
            On_PlayerDied(controller);
    }

    public void CompleteLevel()
    {
#if UNITY_WEBGL
        ((WebSummary)PopupCanvas.instance.webSummaryView).Initalize();
        PopupCanvas.instance.webSummaryView.Show(null);
        return;
#endif
        switch(GlobalSingleton.mode)
        {
            case GameMode.Create:
                if(isPublishingLevel)
                {
                    //LevelViewer.instance.ShowPublishLevel();
                    ((LevelPublisher)PopupCanvas.instance.publishView).Initalize();
                    PopupCanvas.instance.publishView.Show(()=>{
                        PopupCanvas.instance.CloseCanvas();
                    }, false, ()=>
                    {
                        GameMaster.instance.ExitPlayMode();
                        LevelBuilder.instance.SetRoomIndex(LevelBuilder.instance.GetFirstRoomIndex());
                        this.Delay1Frame(()=>
                        {   
                            ThumbnailCamera.instance.Snap();
                            ((LevelPublisher)PopupCanvas.instance.publishView).levelPreview.FromBytes(ThumbnailCamera.instance.GetLargeThumbnail());
                        });
                    });

                    isPublishingLevel = false;
                }
                else
                {
                    TransitionSingleton.instance.CloseBlinds(LevelBuilder.instance.GetCurrentColors().main, false, ()=>{
                        //LevelBuilder.instance.SetRoomIndex(LevelBuilder.instance.GetFirstRoomIndex());
                        ExitPlayMode();
                        TransitionSingleton.instance.OpenBlinds();
                    });
                }
            break;
            case GameMode.FreePlay:
                DialogCanvas.instance.ShowLoading();

                int totalNumberOfJumps = jumps.Sum();

                double playerTime = Stopwatch.instance.GetTotal();
                LevelAPI.LogLevelComplete(currentlyPlayingLevel.PublishedLevelMeta.LevelId, currentlyPlayingLevel.PublishedLevelMeta.CreatorUserId, playerTime, deaths, totalNumberOfJumps, (success)=>
                {
                    //LevelViewer.instance.ShowSummary(playerTime, currentlyPlayingLevel);
                    ((LevelSummary)PopupCanvas.instance.summaryView).Initalize(playerTime, currentlyPlayingLevel);
                    ((LevelSummary)PopupCanvas.instance.summaryView).ShowLikeView(true);
                    PopupCanvas.instance.summaryView.Show(null);
                });
            break;
            case GameMode.Campaign:

				int world = (int)currentlyPlayingLevel.campaignInfo.world;
				int index = currentlyPlayingLevel.campaignInfo.campaignIndex;

				bool save = false;
				if(Stopwatch.instance != null)
				{
                    Highscore newScore = new Highscore(Stopwatch.instance.GetTotal());
                    Highscore oldScore = new Highscore(SaveManager.currentSave.campaignProgress[world][index].seconds, SaveManager.currentSave.campaignProgress[world][index].milliseconds);
					if(newScore.IsLowerThan(newScore))
					{
						SaveManager.currentSave.campaignProgress[world][index].seconds = newScore.Seconds;
                        SaveManager.currentSave.campaignProgress[world][index].milliseconds = newScore.Milliseconds;
						save = true;
					}

                    //Upload score!
                    HighscoreAPI.UploadScore(newScore, world, index);
				}

                if(!SaveManager.currentSave.campaignProgress[world][index].beaten)
                {
                    SaveManager.currentSave.campaignProgress[world][index].beaten = true;   
                    save = true;
                }

				if(save)
                {
                    SaveManager.currentSave.needsToUpdateServer = true;
                    SaveManager.Save();
                }

                //LevelViewer.instance.ShowCampaignSummary(currentlyPlayingLevel);
                PopupCanvas.instance.campaignSummaryView.Show(null);
            break;
        }
    }

    public void ReplayLevel()
    {
        deaths = 0;

        if (Stopwatch.instance != null)
            Stopwatch.instance.ResetAll();

        transitionPointOfNoReturnReached = false;

        ResetLevel();
        LevelBuilder.instance.SetRoomIndex(LevelBuilder.instance.GetFirstRoomIndex());
        PlayLevel();
    }

    public void TestLevel()
    {
        deaths = 0;
        jumps = new int[4];

        LevelBuilder.instance.SaveRoom();
        //SaveManager.SaveWorkInProgress(); //remove autosave.
        PlayLevel();
        LevelBuilder.instance.PlayMusic();
    }

    public void AttemptPublishLevel()
    {
        LevelBuilder.instance.SaveRoom();

        if(LevelBuilder.instance.GetCost() > Globals.gameConstants.blockBudget)
        {
            new Dialog(TranslationKey.Editor_BuildLimit_Error_Title, TranslationKey.Editor_BuildLimit_Error_Body)
            .AddNeutralButton(TranslationKey.Generic_Ok, null).Show();
            return;
        }

        TransitionSingleton.instance.CloseBlinds(LevelBuilder.instance.GetCurrentColors().main, false, () =>
        {
            LevelBuilder.instance.SetRoomIndex(LevelBuilder.instance.GetFirstRoomIndex());
            isPublishingLevel = true;
            roomsCleared = 0;
            LevelBuilder.instance.PlayMusic();


            TransitionSingleton.instance.ShowFlyByText(TranslationKey.Publish_BeatLevelToUploadFlyBy, SaveManager.currentSave.language, 1f, false, null, ()=> {
                
                PlayLevel();
                TransitionSingleton.instance.OpenBlinds();
            });
        });
    }

    public void GoToNextRoom()
    {
        LevelBuilder.instance.SetRoomIndex(LevelBuilder.instance.GetNextFilledRoomIndex());
        PlayLevel();
        SuspensionManager.Suspend(true);

        PlayerSpawnPoint spawn = GameObject.FindObjectOfType<PlayerSpawnPoint>();

        string location = "Room";
        switch(GameMaster.instance.GetCurrentMode())
        {
            case GameMode.Campaign:
                int levelNumber = ((int)GameMaster.instance.currentlyPlayingLevel.campaignInfo.world * 12) + GameMaster.instance.currentlyPlayingLevel.campaignInfo.campaignIndex + 1;
                location = levelNumber.ToString();
                break;
            case GameMode.FreePlay:
                location = GameMaster.instance.currentlyPlayingLevel.PublishedLevelMeta.Name;
                break;
        }

        roomsCleared++;

        TransitionSingleton.instance.ShowFlyByText($"{location} - {roomsCleared+1}", SaveManager.currentSave.language, false, 0.3f, false, null, () => {
            if (spawn != null)
                TransitionHole.instance.Open(spawn.transform, ()=>{
                    SuspensionManager.Suspend(false);
                });
            else
                TransitionHole.instance.Open(null, ()=>{
                    SuspensionManager.Suspend(false);
                });
        });
    }

    public void PlayLevel()
    {
        isPlaying = true;

        startFrame = Time.frameCount;
        hasStartedRunning = false;

        roomsCleared = LevelBuilder.instance.GetCurrentRoomIndex()-1;

        if(On_EnterPlayMode != null)
            On_EnterPlayMode();

        Bullet.ClearBulletList();

        transitionPointOfNoReturnReached = false;
    }

    public void ExitPlayMode()
    {
        roomsCleared = 0;
        isPlaying = false;
        isPublishingLevel = false;

        if(On_ExitPlayMode != null)
            On_ExitPlayMode();

        Time.timeScale = 1f;

        Bullet.ClearBulletList();

        LevelBuilder.instance.PlayMusic();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
    
    public void ResetLevel(bool manualReset = false)
    {
        if (transitionPointOfNoReturnReached)
            return;

        jumps[LevelBuilder.instance.GetCurrentRoomIndex()] = 0;

        startFrame = Time.frameCount;
        hasStartedRunning = false;

        On_LevelReset?.Invoke(manualReset);

        Time.timeScale = 1f;

        Bullet.ClearBulletList();

        transitionPointOfNoReturnReached = false;
    }

    public GameMode GetCurrentMode()
    {
        return GlobalSingleton.mode;
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public bool HasStartedRunning()
    {
        return hasStartedRunning;
    }

    public int GetStartFrame()
    {
        return startFrame;
    }

    public bool TransitionPointOfNoReturnReached()
    {
        return transitionPointOfNoReturnReached;
    }

    public void SetTransitionPointOfNoReturnReached()
    {
        transitionPointOfNoReturnReached = true;
    }

    public bool IsAttemptingToPublishLevel()
	{
		return isPublishingLevel;
	}

    public void LoadLevel(SerializableLevel level)
    {
        roomsCleared = 0;
        deaths = 0;
        jumps = new int[4];

        LevelSerializer.Load(level);
            
        LevelBuilder.instance.SetMusic((Music)level.musicIndex);
        LevelBuilder.instance.PlayMusic();
    }

    public bool OpinionCached()
    {
        if(currentlyPlayingLevel == null || currentlyPlayingLevel.PublishedLevelMeta == null)
            return false;

        if(this.levelUserStats == null)
            return false;

        if(this.levelUserStats.LevelId != currentlyPlayingLevel.PublishedLevelMeta.LevelId)
            return false;

        return true;
    }

    public void ChangeOpinion(LevelOpinion opinion)
    {
        levelUserStats.Opinion = opinion;
    }

    public void GetUserStats(System.Action<bool, LevelUserStats> onComplete)
    {
        //If we are currently playing a level that has been published.
        if(currentlyPlayingLevel == null || currentlyPlayingLevel.PublishedLevelMeta == null)
        {
            onComplete?.Invoke(false, null);
            return;
        }

        if(this.levelUserStats != null)
        {
            if(this.levelUserStats.LevelId == currentlyPlayingLevel.PublishedLevelMeta.LevelId)
            {
                onComplete?.Invoke(true, levelUserStats);
                return;
            }
        }

        LevelAPI.LoadUserStats(currentlyPlayingLevel.PublishedLevelMeta.LevelId, (success, levelUserStats)=>
        {
            if(!success)
            {
                onComplete?.Invoke(false, null);
                return;
            }

            this.levelUserStats = levelUserStats;
            onComplete?.Invoke(true, levelUserStats);
        });
    }

    private void OnPlayerJumped(FSM_CharacterController controller)
    {
        jumps[LevelBuilder.instance.GetCurrentRoomIndex()]++;
    }

    private void OnEnable()
    {
        Jumping.OnPlayerJumped += OnPlayerJumped;
    }

    private void Unsubscribe()
    {
        Jumping.OnPlayerJumped -= OnPlayerJumped;
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}
