using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FollowButton : MonoBehaviour
{
    public TextMeshProUGUI buttonText;

    private bool isFollowed;
    private string userId;

    public void Initalise(bool isFollowed, string userId)
    {
        this.isFollowed = isFollowed;
        this.userId = userId;

        UpdateButton();
    }

    private void UpdateButton()
    {
        buttonText.Translate(this.isFollowed ? TranslationKey.UnfollowButton_Title : TranslationKey.FollowButton_Title, SaveManager.currentSave.language, FontType.Stylized, false);
    }

    public void OnClick()
    {
		ProfileView followerView = (ProfileView)PopupCanvas.instance.profileView;

        DialogCanvas.instance.ShowLoading();
        if(!isFollowed)
        {
            FollowerAPI.FollowUser(userId, (success)=>
            {
                DialogCanvas.instance.HideLoading();
                if(!success)
                {
                    return;
                }

				followerView.followerCount += 1;
				followerView.followerCountTextmesh.text = followerView.followerCount < 0  ? "?" : followerView.followerCount.ToString();

                isFollowed = true;
                UpdateButton();
            });
        }
        else
        {
            FollowerAPI.UnfollowUser(userId, (success)=>
            {
                DialogCanvas.instance.HideLoading();
                if(!success)
                {
                    return;
                }

				followerView.followerCount -= 1;
				followerView.followerCountTextmesh.text = followerView.followerCount < 0  ? "?" : followerView.followerCount.ToString();

                isFollowed = false;
                UpdateButton();
            });
        }
    }
}
