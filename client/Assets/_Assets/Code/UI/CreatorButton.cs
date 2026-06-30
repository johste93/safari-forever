using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatorButton : MonoBehaviour
{
    private string creatorUserId;

    public void SetCreatorUserId(string userId)
    {
        this.creatorUserId = userId;
    }

    public void OnClick()
    {
        if(string.IsNullOrWhiteSpace(creatorUserId))
        {
            Debug.LogError("Creator user id not set!");
            return;
        }

        PopupView currentView = PopupCanvas.instance.GetActivePopup();

        DialogCanvas.instance.ShowLoading();
        ((ProfileView)PopupCanvas.instance.profileView).Initalize(creatorUserId, (success)=>
        {
            DialogCanvas.instance.HideLoading();
            if(!success)
                return;

            currentView.Close(false, ()=>{
                PopupCanvas.instance.profileView.Show(()=>{
                    currentView.Show(null);
                    if(currentView.GetType() == typeof(LevelSummary))
                        ((LevelSummary)currentView).ShowShareView(true);
                });
            });
        });
        
    }
}
