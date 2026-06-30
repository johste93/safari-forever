using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Newtonsoft.Json;

public class LevelSummary : PopupView
{   
    public LevelStatViewer levelStatViewer;
    
    public RectTransform likeView;
    public CanvasGroup likeViewCanvasGroup;
    public RectTransform shareView;
    public CanvasGroup shareViewCanvasGroup;
    public LevelPreview levelPreview;

    public UIToggleButton likeButton;
    public UIToggleButton dislikeButton;

    public CanvasGroup opinionButtons;
    public ShareCodeButton shareCodeButton;

    public CreatorButton creatorButton;
	private Tween likeViewTween;
    private Tween shareViewTween;
    private Level level;

    public void Initalize(double playertime, Level level)
    {
        levelPreview.Reset();
        levelStatViewer.Reset();
        this.level = level;
        levelPreview.SetName(level.PublishedLevelMeta.Name);

        creatorButton.SetCreatorUserId(level.PublishedLevelMeta.CreatorUserId);

        int averageNumberOfDeaths = Mathf.FloorToInt((float)level.PublishedLevelMeta.Deaths / (float)level.PublishedLevelMeta.Wins);
        
        shareCodeButton.shareUrl = level.PublishedLevelMeta.ShareUrl;

        //Load Opinion
        ToggleOpinionButtons(false);
        LoadUserStats((canEdit)=>
        {
            ToggleOpinionButtons(canEdit);
            DialogCanvas.instance.HideLoading();
        });

        levelPreview.LoadImage(level.PublishedLevelMeta.LevelId, ()=>{});

        levelStatViewer.PullStats(level.PublishedLevelMeta.LevelId, playertime, (success)=>{});
    }

    private void ToggleOpinionButtons(bool b)
    {
        opinionButtons.alpha = b ? 1f : 0.25f;
        opinionButtons.blocksRaycasts = b;
    }

    public override void Show(System.Action onExit, bool instant = false, System.Action onComplete = null)
    {
        PopupCanvas.instance.background.DOKill();
        PopupCanvas.instance.background.DOColor(level.serializableLevel.palette.main.color, 0.3f);
        base.Show(onExit, instant, onComplete);
    }

    private void LoadUserStats(System.Action<bool> callback)
    {
        GameMaster.instance.GetUserStats((success, levelUserStats)=>
        {
            if(!success)
            {
                callback(false);
                return;
            }

            switch(levelUserStats.Opinion)
            {
                case LevelOpinion.Liked:
                    likeButton.SetOn(true);
                    dislikeButton.SetOn(false);
                    levelStatViewer.likeModifier = 1;
                    levelStatViewer.alreadyLiked = true;
                    levelStatViewer.UpdateLikeModifier();
                break;
                case LevelOpinion.Disliked:
                    likeButton.SetOn(false);
                    dislikeButton.SetOn(true);
                    levelStatViewer.likeModifier = 0;
                    levelStatViewer.alreadyLiked = false;
                    levelStatViewer.UpdateLikeModifier();
                break;
                default:
                case LevelOpinion.None:
                    likeButton.SetOn(false);
                    dislikeButton.SetOn(false);
                    levelStatViewer.likeModifier = 0;
                    levelStatViewer.alreadyLiked = false;
                    levelStatViewer.UpdateLikeModifier();
                break;
            }

            callback(true);
        });
    }

    public void OnClickContinue()
    {
        SceneLoader.Load(SafariScene.Menu);
    }

    public void OnConfirmLike()
    {
        if(likeButton.IsOn())
            LikeLevel();

        if(dislikeButton.IsOn())
            DislikeLevel();

        if(GameMaster.instance.OpinionCached() && !likeButton.IsOn() && !dislikeButton.IsOn())
            ClearOpinion();

        ShowShareView(false);
    }

    private void ClearOpinion()
    {
        Level level = GameMaster.instance.currentlyPlayingLevel;

        //If we are currently playing a level that has been published.
        if(level == null || level.PublishedLevelMeta == null)
            return;

        LevelAPI.ClearLevelOpinion(level.PublishedLevelMeta.LevelId, level.PublishedLevelMeta.CreatorUserId, (success)=>
        {
            if(!success)
                return;

            GameMaster.instance.ChangeOpinion(LevelOpinion.None);
        });
    }

    private void LikeLevel()
    {
        Level level = GameMaster.instance.currentlyPlayingLevel;

        //If we are currently playing a level that has been published.
        if(level == null || level.PublishedLevelMeta == null)
            return;

        LevelAPI.LikeLevel(level.PublishedLevelMeta.LevelId, level.PublishedLevelMeta.CreatorUserId, (success)=>
        {
            if(!success)
                return;

            GameMaster.instance.ChangeOpinion(LevelOpinion.Liked);
        });
    }

    private void DislikeLevel()
    {
        Level level = GameMaster.instance.currentlyPlayingLevel;

        //If we are currently playing a level that has been published.
        if(level == null || level.PublishedLevelMeta == null)
            return;

        LevelAPI.DislikeLevel(level.PublishedLevelMeta.LevelId, level.PublishedLevelMeta.CreatorUserId, (success)=>
        {
            if(!success)
                return;

            GameMaster.instance.ChangeOpinion(LevelOpinion.Disliked);
        });
    }

    public void OnClickReplay()
    {
        Exit(false, ()=>{
            GameMaster.instance.ReplayLevel();
        });
    }

    private void OnEnable()
    {
        //likeButton.On_Toggle += On_ToggleLike;
        //dislikeButton.On_Toggle += On_ToggleDislike;
    }

    public void On_ToggleLike()
    {
        if(likeButton.IsOn())
            levelStatViewer.likeModifier = 1;
        else
            levelStatViewer.likeModifier = 0;
            
        levelStatViewer.UpdateLikeModifier();
    }

    public void On_ToggleDislike()
    {
        levelStatViewer.likeModifier = 0;

        levelStatViewer.UpdateLikeModifier();
    }

    public void ShowLikeView(bool instant)
    {
        HideShareView(instant,()=>{
            float duration = 0.4f ;
            likeView.gameObject.SetActive(true);
            likeView.DOAnchorPosY(0f, instant ? 0f: duration).SetEase(Ease.OutQuad);
            likeViewCanvasGroup.DOKill();
            likeViewCanvasGroup.DOFade(1f, duration).SetEase(Ease.OutQuad);
        });
    }

    public void ShowShareView(bool instant)
    {
        HideLikeView(instant,()=>{
            float duration = 0.4f;
            shareView.gameObject.SetActive(true);
            shareView.DOAnchorPosY(0f, instant ? 0f: duration).SetEase(Ease.OutQuad);
            shareViewCanvasGroup.DOKill();
            shareViewCanvasGroup.DOFade(1f, duration).SetEase(Ease.OutQuad);
        });  
    }

    public void HideLikeView(bool instant, System.Action OnComplete)
    {
        float duration = instant ? 0f: 0.3f;
        likeView.DOKill();
        likeView.DOAnchorPosY(-likeView.rect.height*0.25f, duration).SetEase(Ease.InQuad);

        likeViewCanvasGroup.DOKill();
        likeViewCanvasGroup.DOFade(0f, duration).SetEase(Ease.OutQuad);

        likeViewTween = DOVirtual.DelayedCall(duration*0.75f, ()=>{
            likeView.gameObject.SetActive(false);
            OnComplete();
        });
    }

    public void HideShareView(bool instant, System.Action OnComplete)
    {
        float duration = instant ? 0f: 0.3f;
        shareView.DOKill();
        shareView.DOAnchorPosY(-shareView.rect.height*0.25f, duration).SetEase(Ease.InQuad);

        shareViewCanvasGroup.DOKill();
        shareViewCanvasGroup.DOFade(0f, duration).SetEase(Ease.OutQuad);

        likeViewTween = DOVirtual.DelayedCall(duration*0.75f, ()=>{
            shareView.gameObject.SetActive(false);
            OnComplete();
        });
    }

    private void Unsubscribe()
    {
        likeButton.On_Toggle -= On_ToggleLike;
        dislikeButton.On_Toggle -= On_ToggleDislike;
    }

    private void OnDisable()
    {
        likeButton.SetOn(false);
        dislikeButton.SetOn(false);
        Unsubscribe();
    }

	private void OnDestroy()
	{
        Unsubscribe();
		KillAllTweens();
	}

	private void KillAllTweens()
	{
		likeView.DOKill();
		shareView.DOKill();
		likeViewCanvasGroup.DOKill();
		shareViewCanvasGroup.DOKill();
		likeViewTween?.Kill();
        shareViewTween?.Kill();
	}
}
