using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsButton : MonoBehaviour
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
            ((LevelsView)PopupCanvas.instance.levelsView).LoadUserLevels(userId);
            PopupCanvas.instance.levelsView.Show(()=>{
                currentView.Show(null);
            });
        });
    }
}
