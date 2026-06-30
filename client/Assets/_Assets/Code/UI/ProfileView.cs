using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ProfileView : PopupView
{
    public TextMeshProUGUI header;
    public TextMeshProUGUI followerCountTextmesh;
    public TextMeshProUGUI endlessRankTextmesh;
    public TextMeshProUGUI dailyChallengesWonTextmesh;
	public TextMeshProUGUI badgeTextmesh;
	public int followerCount;
    public int endlessRank;
    public int dailyChallengesWon;

    public ShareCodeButton shareCodeButton;
    public GameObject notificationGroup;

    public NameChange nameChange;
    public NotificationButton notificationButton;
    public ProfileLevelWidget profileLevelWidget;
    public FollowingsButton followingsButton;
    public ProfileColorButton profileColorButton;
    public FollowButton followButton;

    public void Initalize(string userId, System.Action<bool> onComplete)
    {
        UserAPI.FetchProfile(userId, (success, profile)=>
        {
            onComplete(success);

            if(!success)
                return;

            Initalize(profile);
        });
    }

    public void Initalize(ProfileResponse profile)
    {
        GlobalSingleton.lastLoadedProfile = profile.UserId;

        badgeTextmesh.text = "";
        profileLevelWidget.widgetHeader.text = "";
		header.rectTransform.anchoredPosition = new Vector3(0, profileLevelWidget.widgetHeader.rectTransform.anchoredPosition.y);
        
        notificationGroup.SetActive(SaveManager.currentSave.IsLocalUser(profile.UserId));
        followingsButton.gameObject.SetActive(SaveManager.currentSave.IsLocalUser(profile.UserId));
        profileColorButton.gameObject.SetActive(SaveManager.currentSave.IsLocalUser(profile.UserId));
        followButton.gameObject.SetActive(!SaveManager.currentSave.IsLocalUser(profile.UserId));

        followingsButton.SetUserId(profile.UserId);
        followButton.Initalise(profile.IsFollowed, profile.UserId);

        shareCodeButton.shareUrl = profile.ShareUrl;

        ColorUtility.TryParseHtmlString(profile.Color, out Color c);
        PopupCanvas.instance.background.DOColor(c, 0.3f);
        
        nameChange.Initalize(profile.Nickname, profile.Identifier, SaveManager.currentSave.IsLocalUser(profile.UserId));
        
        if(profile.BetaAccount)
        {
            badgeTextmesh.text = "β";
            badgeTextmesh.color = new Color(1f, 0.2978002f, 0.2235294f, 1f);
            Vector3 anchoredPosition = badgeTextmesh.rectTransform.anchoredPosition;
            anchoredPosition.x = 28.74f;
            badgeTextmesh.rectTransform.anchoredPosition = anchoredPosition;
            header.rectTransform.anchoredPosition = new Vector3(-anchoredPosition.x*0.5f, profileLevelWidget.widgetHeader.rectTransform.anchoredPosition.y);
        }

        if(profile.AlphaAccount)
        {
            badgeTextmesh.text = "α";
            badgeTextmesh.color = new Color(0.2216981f, 0.678683f, 1f, 1f);
            Vector3 anchoredPosition = badgeTextmesh.rectTransform.anchoredPosition;
            anchoredPosition.x = 37.53f;
            badgeTextmesh.rectTransform.anchoredPosition = anchoredPosition;
            header.rectTransform.anchoredPosition = new Vector3(-anchoredPosition.x*0.5f, profileLevelWidget.widgetHeader.rectTransform.anchoredPosition.y);
        }

        profileLevelWidget.widgetHeader.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, profileLevelWidget.widgetHeader.font, SaveManager.currentSave.language);
        profileLevelWidget.widgetHeader.isRightToLeftText = Localization.IsRightToLeftLanguage(SaveManager.currentSave.language);

        if(SaveManager.currentSave.IsLocalUser(profile.UserId))
            profileLevelWidget.widgetHeader.Translate(TranslationKey.Browser_MyLevels, SaveManager.currentSave.language, FontType.Stylized_Outlined, false);
        else
        {
            bool isRTL = Localization.IsRightToLeftLanguage(Localization.KeyAvailable(TranslationKey.LevelsWidget_Header, SaveManager.currentSave.language) ? SaveManager.currentSave.language : Globals.localizationConstants.defaultLanguage);
            profileLevelWidget.widgetHeader.text = Localization.GetTranslationFormat2(TranslationKey.LevelsWidget_Header, SaveManager.currentSave.language, isRTL ? $"<ltr>{profile.Nickname}</ltr>" : profile.Nickname);
            profileLevelWidget.widgetHeader.isRightToLeftText = isRTL;
        }

        profileLevelWidget.Initalize(profile.UserId, profile.Levels);
        followerCount = profile.FollowerCount;
        followerCountTextmesh.text = followerCount < 0 ? "?" : followerCount.ToString();
        followerCountTextmesh.alignment = Localization.IsRightToLeftLanguage(SaveManager.currentSave.language) ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        followerCountTextmesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, followerCountTextmesh.font, SaveManager.currentSave.language);

        endlessRank = profile.EndlessRank;
        endlessRankTextmesh.text = endlessRank < 0 ? "n/a" : $"{endlessRank}.";
        endlessRankTextmesh.alignment = Localization.IsRightToLeftLanguage(SaveManager.currentSave.language) ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        endlessRankTextmesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, followerCountTextmesh.font, SaveManager.currentSave.language);

        dailyChallengesWon = profile.DailyChallengesWon;
        dailyChallengesWonTextmesh.text = dailyChallengesWon < 0 ? "?" : dailyChallengesWon.ToString();
        dailyChallengesWonTextmesh.alignment = Localization.IsRightToLeftLanguage(SaveManager.currentSave.language) ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        dailyChallengesWonTextmesh.font = Globals.accessibilityConstants.GetFont(SaveManager.currentSave.openDyslexic, FontType.Stylized_Outlined, followerCountTextmesh.font, SaveManager.currentSave.language);
    }

    public void OnClickClose()
    {
        base.Exit();
    }
}
