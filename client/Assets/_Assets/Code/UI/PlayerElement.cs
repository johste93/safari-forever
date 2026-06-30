using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerElement : MonoBehaviour
{
    public Image thumbnailImage;

    public TextMeshProUGUI title;
    public TextMeshProUGUI body;

    private ProfileInfoDTO profileDTO;
    private Tween thisTween;

    public void Initalize(ProfileInfoDTO profileDTO)
    {
        this.profileDTO = profileDTO;

        title.text = profileDTO.Nickname;
        body.text = $"Last Active: {profileDTO.LastActive.LocalDateTime.ToShortDateString()}";

        if(ColorUtility.TryParseHtmlString(profileDTO.Color, out Color color))
            thumbnailImage.color = color;
    }

    public void Initalize(FollowedUserDTO followedUserDTO)
    {
        this.profileDTO = new ProfileInfoDTO(){
            UserId = followedUserDTO.UserId,
            Nickname = followedUserDTO.Nickname,
            Identifier = followedUserDTO.Identifier,
            Color = followedUserDTO.Color,
            LastActive = followedUserDTO.LastActive
        };

        title.text = followedUserDTO.Nickname;
        body.text = $"Last Active: {followedUserDTO.LastActive.LocalDateTime.ToShortDateString()}";

        if(ColorUtility.TryParseHtmlString(followedUserDTO.Color, out Color color))
            thumbnailImage.color = color;
    }

    public void OnClick()
    {
        if(thisTween != null)
            thisTween.Complete();
            
        thisTween = transform.DOPunchScale(new Vector3(-0.05f, -0.2f, 0), 0.3f, 5);

        PopupView currentView = PopupCanvas.instance.GetActivePopup();

        DialogCanvas.instance.ShowLoading();
        ((ProfileView)PopupCanvas.instance.profileView).Initalize(profileDTO.UserId, (success)=>
        {
            DialogCanvas.instance.HideLoading();
            if(!success)
                return;

            SaveManager.currentSave.FetchOnlineProfile((profile)=>
            {
                if(profile == null)
                {
                    DialogCanvas.instance.HideLoading();
                    return;
                }

                System.Action onExit = ()=>{
                
                    if(ColorUtility.TryParseHtmlString(profile.color, out Color color))
                    {
                        PopupCanvas.instance.background.DOKill();
                        PopupCanvas.instance.background.DOColor(color, 0.3f);
                    }

                    PopupCanvas.instance.followingsView.Show(null);
                    
                    PopupCanvas.instance.followingsView.onExit = ()=>
                    {
                        ((ProfileView)PopupCanvas.instance.profileView).Initalize(profile.userId, (success2)=>
                        {
                            DialogCanvas.instance.HideLoading();
                            if(!success2)
                                return;

                            PopupCanvas.instance.profileView.Show(null);

                            PopupCanvas.instance.profileView.onExit = null;
                        });
                        
                    }; 
                };

                PopupCanvas.instance.followingsView.Close(false, ()=>{
                    PopupCanvas.instance.profileView.Show(null, false);
                    PopupCanvas.instance.profileView.onExit = onExit;
                });
            });

            /*
                System.Action onExitCache = PopupCanvas.instance.followingsView.onExit;
                currentView.Close(false, ()=>{
                    
                    PopupCanvas.instance.profileView.Show(()=>{});

                    PopupCanvas.instance.profileView.onExit = ()=>{
                        currentView.Show(null);
                    };

                    currentView.onExit = onExitCache;
                });
            */
        });
    }

    private void KillAllTweens()
    {
        thisTween?.Kill();
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }
}
