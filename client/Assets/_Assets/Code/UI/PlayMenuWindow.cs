using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MandarinDuck.NativeShareDialog;

public class PlayMenuWindow : MonoBehaviour
{
    public RectTransform window;
    private Tween tween;

    public List<CanvasGroup> opinionButtons;
    public List<CanvasGroup> otherButton;

    public UIToggleButton likeButton;
    public UIToggleButton dislikeButton;

    public void Show()
    {
        window.DOKill();
        window.anchoredPosition = new Vector3(0, -400f, 0);

        Canvas.ForceUpdateCanvases();

        window.gameObject.SetActive(true);
        tween = window.DOAnchorPosY(Screen.width <= Screen.height ? 100f : (800f - window.sizeDelta.y) / 2f, 0.3f);

        UpdateOpinionButtons();
    }

    private void UpdateOpinionButtons()
    {
        ToggleOptionButtons(false);
        GameMaster.instance.GetUserStats((success, stats)=>
        {
            if(!success)
                return;

            switch(stats.Opinion)
            {
                case LevelOpinion.Liked:
                    likeButton.SetOn(true);
                    dislikeButton.SetOn(false);
                break;
                case LevelOpinion.Disliked:
                    likeButton.SetOn(false);
                    dislikeButton.SetOn(true);
                break;
                default:
                case LevelOpinion.None:
                    likeButton.SetOn(false);
                    dislikeButton.SetOn(false);
                break;
            }

            ToggleOptionButtons(true);
        });
    }

    public void ShowCreatorProfile()
    {
        Level level = GameMaster.instance.currentlyPlayingLevel;

        if(level == null || level.PublishedLevelMeta == null)
            return;

        if(string.IsNullOrWhiteSpace(level.PublishedLevelMeta.CreatorUserId))
        {
            Debug.LogError("Creator user id not set!");
            return;
        }

        PopupView currentView = PopupCanvas.instance.GetActivePopup();

        DialogCanvas.instance.ShowLoading();
        ((ProfileView)PopupCanvas.instance.profileView).Initalize(level.PublishedLevelMeta.CreatorUserId, (success)=>
        {
            DialogCanvas.instance.HideLoading();
            if(!success)
                return;

            Close();

            PopupCanvas.instance.profileView.Show(()=>{
                PopupCanvas.instance.CloseCanvas();
            });
        });
    }

    public void Share()
    {
        Level level = GameMaster.instance.currentlyPlayingLevel;

        if(level == null || level.PublishedLevelMeta == null)
            return;

        string shareUrl = level.PublishedLevelMeta.ShareUrl;

        if(Application.isEditor)
            shareUrl.CopyToClipboard();

        SocialManager.Share( shareUrl );
    }

    public void UpdateOpinion()
    {
        if(likeButton.IsOn())
            LikeLevel();

        if(dislikeButton.IsOn())
            DislikeLevel();

        if(GameMaster.instance.OpinionCached() && !likeButton.IsOn() && !dislikeButton.IsOn())
            ClearOpinion();
    }

    private void ClearOpinion()
    {
        Level level = GameMaster.instance.currentlyPlayingLevel;

        //If we are currently playing a level that has been published.
        if(level == null || level.PublishedLevelMeta == null)
            return;

        DialogCanvas.instance.ShowLoading();
        ToggleAllButtons(false);

        LevelAPI.ClearLevelOpinion(level.PublishedLevelMeta.LevelId, level.PublishedLevelMeta.CreatorUserId, (success)=>
        {
            DialogCanvas.instance.HideLoading();
            ToggleAllButtons(true);

            if(!success)
            {
                UpdateOpinionButtons();
                return;
            }

            GameMaster.instance.ChangeOpinion(LevelOpinion.None);
        });
    }

    private void LikeLevel()
    {
        Level level = GameMaster.instance.currentlyPlayingLevel;

        //If we are currently playing a level that has been published.
        if(level == null || level.PublishedLevelMeta == null)
            return;

        DialogCanvas.instance.ShowLoading();
        ToggleAllButtons(false);

        LevelAPI.LikeLevel(level.PublishedLevelMeta.LevelId, level.PublishedLevelMeta.CreatorUserId, (success)=>
        {
            DialogCanvas.instance.HideLoading();
            ToggleAllButtons(true);

            if(!success)
            {
                UpdateOpinionButtons();
                return;
            }

            GameMaster.instance.ChangeOpinion(LevelOpinion.Liked);
        });
    }

    private void DislikeLevel()
    {
        Level level = GameMaster.instance.currentlyPlayingLevel;

        //If we are currently playing a level that has been published.
        if(level == null || level.PublishedLevelMeta == null)
            return;

        DialogCanvas.instance.ShowLoading();
        ToggleAllButtons(false);

        LevelAPI.DislikeLevel(level.PublishedLevelMeta.LevelId, level.PublishedLevelMeta.CreatorUserId, (success)=>
        {
            DialogCanvas.instance.HideLoading();
            ToggleAllButtons(true);

            if(!success)
            {
                UpdateOpinionButtons();
                return;
            }

            GameMaster.instance.ChangeOpinion(LevelOpinion.Disliked);
        });
    }

    private void ToggleOptionButtons(bool b)
    {
        foreach(CanvasGroup cG in opinionButtons)
        {
            cG.alpha = b ? 1f : 0.25f;
            cG.blocksRaycasts = b;
        }
    }

    private void ToggleAllButtons(bool b)
    {
        ToggleOptionButtons(b);

        foreach(CanvasGroup cG in otherButton)
        {
            cG.alpha = b ? 1f : 0.25f;
            cG.blocksRaycasts = b;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
        DialogCanvas.instance.FadeOutbackground();
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }

    private void KillAllTweens()
    {
        if(tween != null)
        {
            tween.Kill();
            tween = null;
        }
    }
}
