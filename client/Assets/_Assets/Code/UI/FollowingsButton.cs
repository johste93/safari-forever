using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingsButton : MonoBehaviour
{
    private string userId;

    public void SetUserId(string userId)
    {
        this.userId = userId;
    }

    public void OnClick()
    {
        if(string.IsNullOrWhiteSpace(userId))
        {
            Debug.LogError("userId not set!");
            return;
        }

        PopupView currentView = PopupCanvas.instance.GetActivePopup();
        currentView.Close(false, ()=>
        {
            ((FollowingsView)PopupCanvas.instance.followingsView).Initalize(userId);
            PopupCanvas.instance.followingsView.Show(()=>{
                currentView.Show(null);
            });
        });
    }
}
