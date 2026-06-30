using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ProfileElement : MonoBehaviour
{
    public Image thumbnailImage;
    public Image colorImage;

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
            colorImage.color = color;
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

            currentView.Close(false, ()=>{
                PopupCanvas.instance.profileView.Show(()=>{
                    PopupCanvas.instance.background.DOColor(Camera.main.backgroundColor, 0.3f);
                    currentView.Show(null);
                });
            });
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
