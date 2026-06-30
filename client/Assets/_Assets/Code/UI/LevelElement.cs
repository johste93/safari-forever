using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class LevelElement : MonoBehaviour
{
    public Image thumbnailImage;
    public GameObject bananaBonusIcon;
    public GameObject levelCompletedIcon;

    public TextMeshProUGUI number;
    public TextMeshProUGUI title;
    public TextMeshProUGUI body;
    public TextMeshProUGUI multiplier;

    public GameObject boostButton;
    public TextMeshProUGUI boostAmount;

    private LevelInfoDTO levelInfo;
    private Tween thisTween;
    private Tween backgroundColorTween;

    public TextMeshProUGUI difficulity;

    private MenuLocation exitLocation;

    public void OnClick()
    {
        if(thisTween != null)
            thisTween.Complete();
            
        thisTween = transform.DOPunchScale(new Vector3(-0.05f, -0.2f, 0), 0.3f, 5);

        LevelAPI.Download(levelInfo.LevelId, (level) =>
        {
            if (level == null)
                return;

            PopupView currentView = PopupCanvas.instance.GetActivePopup();
			Color backgroundColor = PopupCanvas.instance.background.color;
            currentView.Close(false, ()=>{
                ((LevelInspector)PopupCanvas.instance.inspectView).Initalize(level, exitLocation);
                PopupCanvas.instance.inspectView.Show(()=>{
					backgroundColorTween = PopupCanvas.instance.background.DOColor(backgroundColor, 0.3f);
                    currentView.Show(null);
                });
            });
        });
    }

    public void BoostLevel()
    {
        DialogCanvas.instance.ShowBoostWindow(this, (coinsInvested)=>
        {
            UpdateBoostedAmountText(coinsInvested);
        });
    }

    public void Initalize(int index, LevelInfoDTO levelInfo, MenuLocation exitLocation = MenuLocation.MainMenu, bool showLevelComplete = true, bool mayShowBoostButton = false)
    {  
        this.levelInfo = levelInfo;
        this.exitLocation = exitLocation;

        bool levelCompleted = SaveManager.currentSave.levelsPlayed.Contains(levelInfo.LevelId);
        bool madeByLocalUser = SaveManager.currentSave.IsLocalUser(levelInfo.CreatorUserId);
        
        boostButton.SetActive(madeByLocalUser && levelInfo.CanBeBoosted && mayShowBoostButton);
        levelCompletedIcon.SetActive((madeByLocalUser || levelCompleted) && showLevelComplete);
        bananaBonusIcon.SetActive(!madeByLocalUser && !levelCompleted && levelInfo.RewardMultipler > 1 && !levelCompletedIcon.activeInHierarchy);
        boostAmount.gameObject.SetActive(mayShowBoostButton);

        number.gameObject.SetActive(index > 0);
        number.text = $"{index}.";
        
        gameObject.name = $"LevelElement ({levelInfo.Name})";

        title.text = levelInfo.Name;
        body.text = $"By {levelInfo.Creator}";
        multiplier.text = $"{levelInfo.RewardMultipler}x<sprite name=\"Bananas_0\" color=#FFFFFFFF>";
        
        LocalizeDifficulty(levelInfo.Difficulty, difficulity);

        UpdateBoostedAmountText(levelInfo.CoinsInvested);

        Sprite preview;
        if(levelInfo.MiniThumbnail == null)
        {
            preview = levelInfo.Thumbnail.ToSpriteFromGIF();
        }
        else
        {
            preview = levelInfo.MiniThumbnail.ToSpriteFromGIF();
        }
        
		if(preview == null)
		{
			Debug.LogError("Unable to Parse Image");
			return;
		}

		SetImage(preview);
    }

    private void LocalizeDifficulty(Difficulty difficulty, TextMeshProUGUI textMesh)
    {
        switch(difficulty)
        {
            case Difficulty.Unrated:
                textMesh.Translate(TranslationKey.Difficulty_Unrated, SaveManager.currentSave.language, FontType.Regular, true);
                break;
            case Difficulty.Beginner:
                textMesh.Translate(TranslationKey.Difficulty_Beginner, SaveManager.currentSave.language, FontType.Regular, true);
                break;
            case Difficulty.Intermediate:
                textMesh.Translate(TranslationKey.Difficulty_Intermediate, SaveManager.currentSave.language, FontType.Regular, true);
                break;
            case Difficulty.Expert:
                textMesh.Translate(TranslationKey.Difficulty_Expert, SaveManager.currentSave.language, FontType.Regular, true);
                break;
            case Difficulty.Savant:
                textMesh.Translate(TranslationKey.Difficulty_Savant, SaveManager.currentSave.language, FontType.Regular, true);
                break;
        }
    }

    public void SetImage(Sprite thumbnail)
    {
        thumbnailImage.enabled = true;
        thumbnailImage.sprite = thumbnail;
    }

    public void UpdateBoostedAmountText(int coinsInvested)
    {
        boostAmount.text = $"+{coinsInvested}";
    }

    public LevelInfoDTO GetLevel()
    {
        return levelInfo;
    }

    private void KillAllTweens()
    {
        thisTween?.Kill();
        backgroundColorTween?.Kill();
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }	
}
